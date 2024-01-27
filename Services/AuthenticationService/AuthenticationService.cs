using DoAn4.DTOs.AuthenticationDTOs;
using DoAn4.Interfaces;
using DoAn4.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DoAn4.DTOs.UserDTO;
using System.Security.Authentication;
using System.Globalization;
using static System.Net.WebRequestMethods;


namespace DoAn4.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {       
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        
        private readonly IUserOTPRepository _userOTPRepository;

        public AuthenticationService(IUserOTPRepository userOTPRepository, IConfiguration configuration, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            
            _userOTPRepository = userOTPRepository;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Người dùng không tồn tại" }
                };
            }           

            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Mật khẩu sai" }
                };

            }
            
            var accessToken = GenerateAccessToken(user);

            var refreshToken = GenerateRefreshToken();
            
            //lấy giờ theo múi giờ đông dương GMT+7
            var gmt7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var refTokenExpiresAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JWT:RefreshTokenExpirationDays")), gmt7);

            var accTokenExpiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JWT:AccessTokenExpirationMinutes"));
            accTokenExpiresAt = TimeZoneInfo.ConvertTimeFromUtc(accTokenExpiresAt, gmt7);
            var refresh = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                Token = refreshToken,
                UserId = user.UserId,
                ExpiresAt = refTokenExpiresAt,
                IsRevoked = false
            };
            await _refreshTokenRepository.AddRefreshTokenAsync(refresh);     
        
            return new AuthenticationResult
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new InfoUserDTO
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Fullname = user.Fullname,
                    Avatar = user.Avatar,
                    Gender =user.Gender,
                    CoverPhoto =user.CoverPhoto,
                    DateOfBirth = user.DateOfBirth,                   
                    Address =user.Address,                   
                    CreateAt = user.CreateAt
                }

            };
        }

        public async Task<AuthenticationResult> RefreshAccesstokenTokenAsync(string refreshToken, string accessToken)
        {
            var validatedToken = GetPrincipalFromToken(accessToken);       
            // Kiểm tra token đã hết hạn hay chưa
            if (validatedToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Token không hợp lệ" } };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims
                    .Single(x => x.Type == JwtRegisteredClaimNames.Exp)
                    .Value);

            // Lấy múi giờ của Việt Nam
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Chuyển đổi thời gian từ UNIX timestamp sang múi giờ Việt Nam
            DateTime expiryDateTimeLocal = TimeZoneInfo.ConvertTimeFromUtc(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix),
                timeZone
            );

            if (expiryDateTimeLocal > TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")))
            {
                return new AuthenticationResult { Errors = new[] { "AccessToken vẫn chưa hết hạn" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(jti);
            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "RefreshToken không tồn tại" } };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiresAt)
            {
                return new AuthenticationResult { Errors = new[] { "RefreshToken đã hết hạn" } };
            }

            if (storedRefreshToken.IsRevoked)
            {
                return new AuthenticationResult { Errors = new[] { "RefreshToken đã bị thu hồi" } };
            }

            if (storedRefreshToken.Token != refreshToken)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };
            }

            // Tạo mới access token và refresh token
            var user = await _userRepository.GetUserByIdAsync(storedRefreshToken.UserId);
            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            var gmt7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var refTokenExpiresAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JWT:RefreshTokenExpirationDays")), gmt7);

            // Cập nhật refresh token mới trong database
            var newRefresh = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.UserId,
                ExpiresAt = refTokenExpiresAt
            };
            await _refreshTokenRepository.AddRefreshTokenAsync(newRefresh);

            // Cập nhật trạng thái của refresh token cũ
            storedRefreshToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateRefreshTokenAsync(storedRefreshToken);

            return new AuthenticationResult
            {
                Success = true,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<ResultRespone> RegisterAsync(UserRegisterDTO request)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);

            if (existingUser != null)
            {
                throw new AuthenticationException("Email đã tồn tại");
            }           

            var userOTP = await _userOTPRepository.GetUserOTPByEmail(request.Email);
            if (userOTP == null || userOTP.Token != request.Token)
            {
                throw new AuthenticationException("OTP không hợp lệ");
            }

            CreatedPasswordHash(request.Password, out byte[] passwordHash, out byte[] salt);
            var formatCurrentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                PasswordSalt = salt,
                PasswordHash = passwordHash,
                Fullname = request.Fullname,
                Avatar = "https://res.cloudinary.com/dxrrctr3z/image/upload/v1685984428/defaultAvatar_omcl4a.png",
                CoverPhoto = "https://res.cloudinary.com/dxrrctr3z/image/upload/v1685984430/coverPhotoDefault_jsgarx.jpg",
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                Address = request.Address,
                CreateAt = DateTime.ParseExact(formatCurrentTime, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                IsEmailVerified = true
            };

            await _userRepository.CreateUserAsync(newUser);
            await _userOTPRepository.DeleteUserOTP(request.Email);

            return new ResultRespone { Status = 200 };
        }

        public async Task<bool> LogoutAsync(string refreshToken, string accessToken)
        {
            var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);

           

            if (storedRefreshToken != null)
            {
                await _refreshTokenRepository.DeleteRefreshTokenAsync(storedRefreshToken.Token);
            }

            

            return storedRefreshToken != null ;
        }

        public async Task<ReadIdUserFromToken> GetIdUserFromAccessToken(string token)
        {
            var claimsPrincipal = GetPrincipalFromToken(token);
            if (claimsPrincipal == null)
            {
                return null;
            }

            var claims = claimsPrincipal.Claims;

            var userIdString = claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            var fullname = claims.FirstOrDefault(c => c.Type == "fullname")?.Value;
            var email = claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var avatar = claims.FirstOrDefault(c => c.Type == "avatar")?.Value;
            var coverPhoto = claims.FirstOrDefault(c => c.Type == "coverPhoto")?.Value;
            
            var expiresAtUnixTime = long.Parse(claims.FirstOrDefault(c => c.Type == "exp")?.Value);

            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expiresAtUnixTime).ToLocalTime().DateTime;
            var currentUtcTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));


            if (currentUtcTime > expiresAt)
            {
                return null;
            }

            else if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return null;
            }
            else
            {
                return new ReadIdUserFromToken
                {
                    UserId = userId,
                    FullName = fullname,
                    Email = email,
                    Avatar = avatar,
                    CoverPhoto = coverPhoto
                };
            }

        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"])),
                    ValidIssuer = _configuration["JWT:ValidIssuer"],
                    ValidAudience = _configuration["JWT:ValidAudience"],
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                if (!(validatedToken is JwtSecurityToken jwtToken))
                    return null;

                else if (!jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return claimsPrincipal;
            }
            catch
            {
                // Return null if the token is not valid
                return null;
            }
        }

        private string GenerateAccessToken(User user)
        {
            // Lấy thông tin cấu hình từ appsetting.json

            var secret = _configuration["JWT:Secret"];
            var issuer = _configuration["JWT:ValidIssuer"];
            var audience = _configuration["JWT:ValidAudience"];
            var accessTokenExpirationMinutes = int.Parse(_configuration["JWT:AccessTokenExpirationMinutes"]);

            // Tạo danh sách các claim cho mã JWT
            var claims = new List<Claim>{
                new Claim("userId", user.UserId.ToString()),
                new Claim("fullname", user.Fullname),
                new Claim("email", user.Email),
                new Claim("avatar",user.Avatar),
                new Claim("coverPhoto",user.CoverPhoto)
            };

            // Tạo key từ secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            
            //lấy giờ hiện tại theo GMT +7
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
            var expiresAt = localTime.AddMinutes(accessTokenExpirationMinutes);

            // Tạo mã JWT
            var token = new JwtSecurityToken
                (
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: expiresAt,
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha512)
                );

            // Trả về mã JWT dưới dạng string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[255];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private void CreatedPasswordHash(string password, out byte[] passwordHash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }
    }
}
