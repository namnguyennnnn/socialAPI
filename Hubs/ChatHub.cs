using DoAn4.Interfaces;
using DoAn4.Services.AuthenticationService;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace DoAn4.Hubs
{
    public class ChatHub : Hub
    {

        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IUserRepository _userRepository;
        
        private readonly IAuthenticationService _authenticationService;
        public ChatHub(IAuthenticationService authenticationService, IMessageRepository messageRepository, IConversationRepository conversationRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _userRepository = userRepository;
            
            _authenticationService = authenticationService;
        }
        public async Task SendMessage(string token,string groupId, Guid recipientId, string content)
        {
            var sender = await _authenticationService.GetIdUserFromAccessToken(token);
            var recipient = await _userRepository.GetUserByIdAsync(recipientId);

            if (sender == null || recipient == null)
            {
                throw new Exception("Invalid sender or recipient.");
            }


            var conversation = await _conversationRepository.GetConversation(sender.UserId, recipientId);
            if (conversation == null)
            {
                conversation = await _conversationRepository.CreateConversation(sender.UserId, recipientId);
            }
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var utcNow = DateTime.UtcNow;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);

            var message = new Models.Message
            {
                MessageId = Guid.NewGuid(),
                ConversationsId = conversation.ConversationsId,
                SenderId = sender.UserId,
                RecipientId = recipient.UserId,
                Content = content,
                CreatedAt = localTime
            };

            await _messageRepository.SendMessage(message);
            
            // Gửi tin nhắn tới client thông qua SignalR
            await Clients.Group(groupId).SendAsync("ReceiveMessage", message.SenderId, message.MessageId, message.RecipientId, content,"b");
            await Clients.Group(sender.UserId.ToString()).SendAsync("ReceiveMessage", message.SenderId, message.MessageId, message.RecipientId, content,"y");
        }
        public async Task SendMessageToGroup(string ConversationsId )
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, ConversationsId);
            await base.OnConnectedAsync();
        }

    }
}
