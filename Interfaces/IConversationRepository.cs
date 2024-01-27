using DoAn4.Models;
using Microsoft.VisualBasic;

namespace DoAn4.Interfaces
{
    public interface IConversationRepository
    {
        Task<Conversations> CreateConversation(Guid userId1, Guid userId2);
        Task<Conversations> GetConversation(Guid userId1, Guid userId2);
        Task<Conversations> GetConversationById(Guid conversationId);
        Task<List<Conversations>> GetAllConversations(Guid userId );
        Task<bool> DeleteConversation(Conversations conversation);

    }
}
