
using DoAn4.DTOs.UserDTO;
using DoAn4.Services.AuthenticationService;
using DoAn4.Services.UserOTPService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {


        private readonly IAuthenticationService _authenticationService;
        private readonly IUserOTPService _userOTPService;


        public AccountController(IUserOTPService userOTPService, IAuthenticationService authenticationService)
        {

            _userOTPService = userOTPService;
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO request)
        {
            var result = await _authenticationService.LoginAsync(request.Email, request.Password);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result);

        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO request)
        {
            try
            {
                var result = await _authenticationService.RegisterAsync(request);

                if (result == null)
                {
                    return BadRequest();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        [HttpPost("getOTP")]
        public async Task<IActionResult> SendOTP([FromBody] string email)
        {
            try
            {
                var result = await _userOTPService.GenerateOTPAndSendToEmail(email);

                if (result == null)
                {
                    return BadRequest();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }




        [HttpPost("logout"), Authorize]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _authenticationService.LogoutAsync(refreshToken, accessToken);
                if (result == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Xảy ra lỗi khi đăng xuất: {ex.Message}");
            }
        }

        [HttpPost("renewToken"), Authorize]
        public async Task<IActionResult> RenewToken(string refreshToken)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _authenticationService.RefreshAccesstokenTokenAsync(refreshToken, accessToken);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken });

        }

    }
}
