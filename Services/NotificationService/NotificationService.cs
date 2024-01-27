using DoAn4.DTOs;
using DoAn4.Interfaces;
using DoAn4.Models;
using DoAn4.Services.AuthenticationService;
using Microsoft.EntityFrameworkCore;

namespace DoAn4.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
       

        private readonly INotifyRepository _notifyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IAuthenticationService _authenticationService;
       

        public NotificationService(IAuthenticationService authenticationService, INotifyRepository notifyRepository, IUserRepository userRepository ,IFriendshipRepository friendshipRepository)
        {
            _notifyRepository = notifyRepository;
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;  
            _authenticationService = authenticationService;
        }





        public async Task SendFriendRequestNotification(Guid sender, Guid receiver)
        {
            var senderInfo = await _userRepository.GetUserByIdAsync(sender);
            var idfriendship = await _friendshipRepository.GetFriendshipByUserIdAndFriendUserId(senderInfo.UserId, receiver);
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var notification = new Notify
            {
                NotifyId = Guid.NewGuid(),
                NotifyContent = $"{senderInfo.Fullname} đã gửi cho bạn một lời mời kết bạn",
                NotifyTime = localTime,
                NotifyType = "friend_request",
                IsRead = false,
                FriendShipId = idfriendship.FriendshipId,
                PostId = Guid.Empty,
                SenderId = sender,
                UserId = receiver
            };

            await _notifyRepository.CreateNotifyAsync(notification);
        }

        public async Task AcceptFriendRequestNotification(Guid curUser, Guid friendShipId)
        {
            var receiverInfo = await _userRepository.GetUserByIdAsync(curUser);
            var friendShip = await _friendshipRepository.GetFriendshipById(friendShipId);
            var sender = friendShip.FriendUserId == curUser ? friendShip.UserId : friendShip.FriendUserId;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var notification = new Notify
            {
                NotifyId = Guid.NewGuid(),
                NotifyContent = $"{receiverInfo.Fullname} đã chấp nhận lời mời kết bạn",
                NotifyTime = localTime,
                NotifyType = "acp_friend_request",
                IsRead = false,
                FriendShipId = friendShipId,
                PostId = Guid.Empty,
                SenderId = curUser,
                UserId = sender,
            };
            await _notifyRepository.CreateNotifyAsync(notification);

           
        }

        public async Task NotifyCommentPost(Guid postId, Guid commentatorId, Guid receiverNotify)
        {
            var commentator = await _userRepository.GetUserByIdAsync(commentatorId);
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var notification = new Notify
            {
                NotifyId = Guid.NewGuid(),
                NotifyContent = $"{commentator.Fullname} đã bình luận về bài viết của bạn",
                NotifyTime = localTime,
                NotifyType = "comment_post",
                IsRead = false,
                FriendShipId = Guid.Empty,
                PostId = postId,
                SenderId = commentatorId,
                UserId = receiverNotify
            };
            try
            {
                await _notifyRepository.CreateNotifyAsync(notification);
            }
            catch (DbUpdateException e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<NotifyDto>> GetAllNotifies(string token)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token);

            if (user == null)
            {
                throw new Exception("Token hết hạn");
            }

            var notifies = await _notifyRepository.GetAllNotifyByIdUser(user.UserId);
            var senderIds = notifies.Select(n => n.SenderId).ToList();
            var senders = await _userRepository.GetListUserAsync(senderIds);

            var notifyDtos = notifies.Select(n => new NotifyDto
            {
                NotifyId = n.NotifyId,
                NotifyContent = n.NotifyContent,
                NotifyTime = n.NotifyTime,
                NotifyType = n.NotifyType,
                IsRead = n.IsRead,
                PostId = n.PostId,
                FriendShipId = n.FriendShipId,
                SenderId = n.SenderId,
                Avatar = senders.FirstOrDefault(s => s.UserId == n.SenderId)?.Avatar
            }).ToList();

            return notifyDtos;
        }
    }
}
