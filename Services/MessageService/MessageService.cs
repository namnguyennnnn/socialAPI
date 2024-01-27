using DoAn4.DTOs;
using DoAn4.DTOs.AuthenticationDTOs;
using DoAn4.Hubs;
using DoAn4.Interfaces;
using DoAn4.Models;
using DoAn4.Services.AuthenticationService;
using Microsoft.AspNetCore.SignalR;

namespace DoAn4.Services.MessageService
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly IAuthenticationService _authenticationService;
        public MessageService(IAuthenticationService authenticationService, IMessageRepository messageRepository,IConversationRepository conversationRepository,IUserRepository userRepository, IHubContext<ChatHub> chatHubContext)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _userRepository = userRepository;
            _chatHubContext = chatHubContext;
            _authenticationService = authenticationService;
        }
        public async Task SendMessage(string token, Guid recipientId, string content)
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

            var message = new Message
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
            await _chatHubContext.Clients.Group(conversation.ConversationsId.ToString()).SendAsync("ReceiveMessage", message);        
        }

        public async Task<List<MessageDto>> GetMessages(string token , Guid friendUserId)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            if(user == null)
            {
                throw new Exception("Token hết hạn");
            }
            return await _messageRepository.GetMessages(user.UserId , friendUserId);
        }

        public async Task<ResultRespone> DeleteMessage(string token, Guid messageId)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            var message = await _messageRepository.GetMessageById(messageId);
            if (user == null)
            {
                throw new Exception("Token hết hạn");
            }
            else if (user.UserId != message.SenderId)
            {
                throw new Exception("Bạn không có quyền xóa");
            }else
            {
                var result = await _messageRepository.DeleteMessagesById(messageId);
                if(result == true)
                {
                    return new ResultRespone {Status = 200 };
                }
                else
                {
                    return new ResultRespone { Status = 400 };
                }
            }
                      
        }
    }
}
