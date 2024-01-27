using DoAn4.DTOs.AuthenticationDTOs;
using DoAn4.DTOs.FriendShipDto;
using DoAn4.DTOs.UserDTO;
using DoAn4.Interfaces;
using DoAn4.Models;
using DoAn4.Services.AuthenticationService;
using DoAn4.Services.NotificationService;
using Mailjet.Client.Resources;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;

namespace DoAn4.Services.FriendshipService
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly INotifyRepository _notifyRepository;

        public FriendshipService(INotifyRepository notifyRepository, IFriendshipRepository friendshipRepository, IUserRepository userRepository, INotificationService notificationService, IAuthenticationService authenticationService)
        {
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _authenticationService = authenticationService;
            _notifyRepository = notifyRepository;
        }


        public async Task<List<InfoUserDTO>> GetListFriendshipPendingAsync(string token)
        {
            var reciver = await _authenticationService.GetIdUserFromAccessToken(token) ?? throw new AuthenticationException("Token đã hết hạn");

            List<FriendshipDto> listfriendships = await _friendshipRepository.GetAllFriendshipPendingAsync(reciver.UserId);

            if (listfriendships == null)
            {
                throw new ArgumentNullException(nameof(listfriendships), "Không có người nào muốn làm bạn với bạn cả");
            }

            List<Guid> friendshipUserIds = await _friendshipRepository.GetAllFriendUseridFromListFriendship(listfriendships) ?? throw new Exception("Có lỗi xảy ra khi lấy danh sách bạn bè");

            List<InfoUserDTO> friendPendingOfUser = await _userRepository.GetListUserAsync(friendshipUserIds);

           
            List<Guid> friendshipIds = listfriendships.Select(f => f.FriendShipId).ToList();

            
            foreach (var friend in friendPendingOfUser)
            {
                
                var friendshipId = friendshipIds[friendPendingOfUser.IndexOf(friend)];
                friend.FriendShipId = friendshipId;
            }

            return friendPendingOfUser;
        }


        public async Task<List<InfoUserDTO>> GetListFriendshipAsync(Guid userId)
        {
           
            List<FriendshipDto> listfriendships = await _friendshipRepository.GetAllFriendshipAsync(userId);

            if (listfriendships == null)
            {
                throw new ArgumentNullException(nameof(listfriendships), "Không có người bạn nào");
            }

            List<Guid> friendshipUserIds = await _friendshipRepository.GetAllFriendUseridFromListFriendshipAsync(userId, listfriendships) ?? throw new Exception("Có lỗi xảy ra khi lấy danh sách bạn bè");

            List<InfoUserDTO> friendOfUser = await _userRepository.GetListUserAsync(friendshipUserIds);

            // Loại bỏ curUser khỏi danh sách bạn bè
            friendOfUser = friendOfUser.Where(u => u.UserId != userId).ToList();

            return friendOfUser;
        }


        public async Task<FriendResultRespone> SendFriendRequest(string token, Guid friendUserId)
        {

            var sender = await _authenticationService.GetIdUserFromAccessToken(token);
            var receiver = await _userRepository.GetUserByIdAsync(friendUserId);

            if (sender == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }
            else if (sender == null || receiver == null)
            {
                throw new AuthenticationException("Người dùng không tồn tại");
            }
            else if (await _friendshipRepository.IsFriendshipExist(sender.UserId, friendUserId))
            {
                throw new Exception("Hai người đã là bạn ");
            }
            else if (await _friendshipRepository.IsFriendshipRequestExit(sender.UserId, friendUserId) == false) 
            {
                throw new Exception("Đã tồn tại yêu cầu kết bạn ");
            }

            var friendship = new Friendship
            {
                FriendshipId = Guid.NewGuid(),
                UserId = sender.UserId,
                FriendUserId = friendUserId,
                FriendStatus = 0
            };
            try
            {
                await _friendshipRepository.AddFriendshipAsync(friendship);
            
                await _notificationService.SendFriendRequestNotification(sender.UserId, friendUserId);
                return new FriendResultRespone
                {
                    FriendShipId = friendship.FriendshipId  ,
                    SenderFriendResquest = friendship.UserId,
                    ReciverFriendResquest =friendship.FriendUserId,
                    FriendStatus = friendship.FriendStatus
                };
            }
            catch(DbUpdateException e)
            {
                throw new Exception(e.Message);
            }                     
        }

        public async Task<ResultRespone> AcceptFriendRequest(string token, Guid friendShipId)
        {

            var receiver = await _authenticationService.GetIdUserFromAccessToken(token);
            var friendship = await _friendshipRepository.GetFriendshipById(friendShipId);
            var sender = friendship.FriendUserId == receiver.UserId ? friendship.UserId : friendship.FriendUserId;
            if (receiver == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }
            else if (receiver == null || sender == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }
            else if (await _friendshipRepository.IsFriendship(friendShipId))
            {
                throw new Exception("Hai người đã là bạn bè");
            }
                     
            await _friendshipRepository.AcceptFriendAsync(receiver.UserId, sender);
            await _notificationService.AcceptFriendRequestNotification(receiver.UserId,friendShipId);

            await _notifyRepository.DeleteNotifyByFriendShipId(friendShipId);

            return new ResultRespone
            {
                Status = 200
            };
        }
         
        public async Task<ResultRespone> DeleteFriendship(string token, Guid friendUserId)
        {
            var curentUser = await _authenticationService.GetIdUserFromAccessToken(token);
            var idFriendship = await _friendshipRepository.GetFriendshipByUserIdAndFriendUserId( curentUser.UserId , friendUserId);
            if (curentUser == null)
            {
                throw new AuthenticationException("Token đã hết hạn");
            }           
            await _friendshipRepository.DeleteFriendshipAsync(idFriendship.FriendshipId);
            return new ResultRespone
            {
                Status = 200
            };
        }

        public async Task<ResultRespone> RejectFriendRequest( Guid friendShipId)
        {                   
            var Friendship = await _friendshipRepository.GetFriendshipById(friendShipId);
            if (Friendship == null)
            {
                throw new AuthenticationException("Hai người chả có quan hệ gì cả");
            }
            try 
            { 
                await _friendshipRepository.DeleteFriendshipAsync(friendShipId);
                await _notifyRepository.DeleteNotifyByFriendShipId(friendShipId);
                return new ResultRespone
                {
                    Status = 200
                };
            }
            catch(DbUpdateException e) 
            {
                throw new Exception(e.Message);
            }                   
        }

       
    }
}
