using DoAn4.DTOs;
using DoAn4.DTOs.AuthenticationDTOs;
using DoAn4.DTOs.UserDTO;

namespace DoAn4.Services.LikeService
{
    public interface ILikeService
    {
        Task<ResultRespone> UpdatePostLikeStatusAsync(string token,Guid postId , LikeDto  request);
        Task<List<InfoUserDTO>> GetInfoUsersLikedAsync(Guid postId);
    }
}
