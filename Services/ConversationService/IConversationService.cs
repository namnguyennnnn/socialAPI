using DoAn4.DTOs;
using DoAn4.DTOs.AuthenticationDTOs;

namespace DoAn4.Services.ConversationService
{
    public interface IConversationService
    {
        Task<List<ConversationDto>> GetAllConversations(string token );
        Task<ResultRespone> DeleteConversation(Guid conversationId);
    }
}
