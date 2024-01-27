using DoAn4.DTOs.AuthenticationDTOs;
using DoAn4.DTOs.FriendShipDto;
using DoAn4.DTOs.UserDTO;


namespace DoAn4.Services.FriendshipService
{
    public interface IFriendshipService
    {
        Task<List<InfoUserDTO>> GetListFriendshipPendingAsync(string token);

        Task<List<InfoUserDTO>> GetListFriendshipAsync(Guid userId);

        

        Task<ResultRespone> DeleteFriendship(string token,Guid friendUserId);

        Task<FriendResultRespone> SendFriendRequest(string token , Guid frienduserid);

        Task<ResultRespone> AcceptFriendRequest(string token, Guid friendShipId);

        Task<ResultRespone> RejectFriendRequest(Guid friendShipId);

    }
}
