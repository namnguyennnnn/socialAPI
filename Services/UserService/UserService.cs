
using DoAn4.Interfaces;
using DoAn4.Services.PostService;
using Microsoft.EntityFrameworkCore;
using DoAn4.Services.AuthenticationService;
using DoAn4.DTOs.UserDTO;
using System.Globalization;
using DoAn4.Models;

namespace DoAn4.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPostService _postService;
        private readonly IFriendshipRepository _friendshipRepository;

        private readonly IAuthenticationService _authenticationService;

        public UserService(IFriendshipRepository friendshipRepository, IAuthenticationService authenticationService,  IUserRepository userRepository, IPostService postService)
        {
            _userRepository = userRepository;
            _postService = postService;           
            _authenticationService = authenticationService;
            _friendshipRepository = friendshipRepository;
        }

        public async Task<InfoUserDTO> GetProfileUserAsync(string token, Guid UserId)
        {
            var curUser = await _authenticationService.GetIdUserFromAccessToken(token);
            var user = await _userRepository.GetUserByIdAsync(UserId);

            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            var friendShip = await _friendshipRepository.GetFriendshipByUserIdAndFriendUserId(curUser.UserId, user.UserId);

            var infoUser = new InfoUserDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                Fullname = user.Fullname,
                Gender = user.Gender,
                Avatar = user.Avatar,
                CoverPhoto = user.CoverPhoto,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                Bio = user.Bio,
                CreateAt = user.CreateAt,
                FriendShipId = Guid.Empty
            };

            if (curUser.UserId == UserId)
            {
                infoUser.FriendStatus = -1;
                infoUser.SenderFriendResquest = Guid.Empty;
                infoUser.ReciverFriendResquest = Guid.Empty;
            }
            else if (friendShip == null )
            {
                infoUser.FriendStatus = 2;
                infoUser.SenderFriendResquest = Guid.Empty;
                infoUser.ReciverFriendResquest = Guid.Empty;
            }            
            else if (curUser.UserId == friendShip.UserId)
            {
                infoUser.FriendShipId = friendShip.FriendshipId;
                infoUser.FriendStatus = friendShip.FriendStatus;
                infoUser.SenderFriendResquest = friendShip.UserId;
                infoUser.ReciverFriendResquest = UserId;
            }
            else
            {
                infoUser.FriendShipId = friendShip.FriendshipId;
                infoUser.FriendStatus = friendShip.FriendStatus;
                infoUser.SenderFriendResquest = UserId;
                infoUser.ReciverFriendResquest = friendShip.FriendUserId;
            }

            return infoUser;
        }


        public async Task<InfoUserDTO> GetProfileUserByTokenAsync(string token)
        {
            var curUser = await _authenticationService.GetIdUserFromAccessToken(token);
            var user = await _userRepository.GetUserByIdAsync(curUser.UserId);
            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }
            var newUserDto = new InfoUserDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                Fullname = user.Fullname,
                Gender = user.Gender,
                Avatar = user.Avatar,
                CoverPhoto = user.CoverPhoto,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                Bio = user.Bio,
                CreateAt = user.CreateAt
            };
            return newUserDto;
        }

        public async Task<InfoUserDTO> UpdateUserProfileAsync(string token, UpdateProFileUserDto? updateProFileUserDto = null)
        {
            var curUser = await _authenticationService.GetIdUserFromAccessToken(token) ?? throw new ArgumentNullException(nameof(token), "Token hết hạn");
            var user = await _userRepository.GetUserByIdAsync(curUser.UserId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Người dùng không tồn tại");
            }
            else if (updateProFileUserDto ==null)
            {
                throw new ArgumentNullException(nameof(updateProFileUserDto), "Không có thông tin cập nhật nào được cung cấp");
            }

            if (updateProFileUserDto?.Avatar != null)
            {
                var imgPath = await _postService.UpdateAvatarAsync(token, updateProFileUserDto.Avatar);
                if (imgPath == null)
                {
                    throw new ArgumentNullException(nameof(imgPath));
                }
                user.Avatar = imgPath;
            }

            if (updateProFileUserDto?.Coverphoto != null)
            {
                var imgPath = await _postService.UpdateCoverPhotoAsync(token, updateProFileUserDto.Coverphoto);
                if (imgPath == null)
                {
                    throw new ArgumentNullException(nameof(imgPath));
                }
                user.CoverPhoto = imgPath;
            }

            if (updateProFileUserDto?.Fullname != null)
            {
                user.Fullname = updateProFileUserDto.Fullname;
            }

            if (updateProFileUserDto?.Address != null)
            {
                user.Address = updateProFileUserDto.Address;
            }

            if (updateProFileUserDto?.DateOfBirth != null)
            {
                var formatTime = TimeZoneInfo.ConvertTimeFromUtc(updateProFileUserDto.DateOfBirth.Value, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                user.DateOfBirth = DateTime.ParseExact(formatTime, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            try
            {
                await _userRepository.UpdateUserAsync(user);
                var UserDto = new InfoUserDTO
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Fullname = user.Fullname,
                    Gender = user.Gender,
                    Avatar = user.Avatar,
                    CoverPhoto = user.CoverPhoto,
                    DateOfBirth = user.DateOfBirth,
                    Address = user.Address,
                    Bio = user.Bio
                };
                return UserDto;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Cập nhật không thành công: " + ex.Message);
            }
        }

    }
}
