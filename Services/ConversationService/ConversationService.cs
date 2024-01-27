using DoAn4.DTOs;
using DoAn4.DTOs.AuthenticationDTOs;
using DoAn4.Interfaces;
using DoAn4.Repositories;
using DoAn4.Services.AuthenticationService;

namespace DoAn4.Services.ConversationService
{
    public class ConversationService:IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserRepository _userRepository;


        public ConversationService(IUserRepository userRepository,IAuthenticationService authenticationService,IConversationRepository conversationRepository, IMessageRepository messageRepository)
        {
            _conversationRepository = conversationRepository;
            _messageRepository = messageRepository;
            _authenticationService = authenticationService;
            _userRepository = userRepository;  
        }

        public async Task<ResultRespone> DeleteConversation(Guid conversationId)
        {
            var conversation = await _conversationRepository.GetConversationById(conversationId);

            if (conversation == null)
            {
               
                return new ResultRespone { Status = 400};
            }
         
            await _messageRepository.DeleteMessagesByConversationId(conversationId);
           
            await _conversationRepository.DeleteConversation(conversation);

            return new ResultRespone { Status = 200 };
        }

        public async Task<List<ConversationDto>> GetAllConversations(string token)
        {
            var user = await _authenticationService.GetIdUserFromAccessToken(token);
            var userId = user.UserId;

            var conversations = await _conversationRepository.GetAllConversations(userId);
            var conversationIds = conversations.Select(c => c.ConversationsId).ToList();

            var lastMessages = await _messageRepository.GetLastMessages(conversationIds);

            var conversationInfoList = new List<ConversationDto>();

            foreach (var conversation in conversations)
            {
                var recipientId = conversation.UserId1 == userId ? conversation.UserId2 : conversation.UserId1;
                var recipient = await _userRepository.GetUserByIdAsync(recipientId);

                var lastMessage = lastMessages.FirstOrDefault(m => m.ConversationsId == conversation.ConversationsId);
                var lastMessageContent = lastMessage != null ? lastMessage.Content : string.Empty;

                var conversationInfo = new ConversationDto
                {   
                    ConversationId = conversation.ConversationsId,
                    Avatar = recipient.Avatar,
                    RecipientId =recipientId,
                    RecipientName = recipient.Fullname,
                    LastMessage = lastMessageContent
                };

                conversationInfoList.Add(conversationInfo);
            }

            return conversationInfoList;
        }

    }
}
