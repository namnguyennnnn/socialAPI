
using DoAn4.DTOs.UserDTO;
using DoAn4.Models;

namespace DoAn4.Services.UserService
{
    public interface IUserService
    {
        Task<InfoUserDTO> GetProfileUserAsync(string token,Guid UserId);

        Task<InfoUserDTO> GetProfileUserByTokenAsync(string token);

        Task<InfoUserDTO> UpdateUserProfileAsync(string token, UpdateProFileUserDto? updateProFileUserDto = null);

       
    }
}
