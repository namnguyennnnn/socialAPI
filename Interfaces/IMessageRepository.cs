using DoAn4.DTOs;

using DoAn4.Models;


namespace DoAn4.Interfaces
{
    public interface IMessageRepository
    {

        Task<Message> SendMessage(Message message);
        Task<List<MessageDto>> GetMessages(Guid curUser, Guid friendUserId);
        Task<Message> GetLastMessage(Guid conversationId);
        Task<List<Message>> GetLastMessages(List<Guid> conversationIds);
        Task<Message> GetMessageById(Guid messageId);
        Task<bool> DeleteMessagesByConversationId(Guid conversationId);
        Task<bool> DeleteMessagesById(Guid messageId);
    }
}
