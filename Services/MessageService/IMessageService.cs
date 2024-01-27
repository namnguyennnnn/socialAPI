using DoAn4.DTOs;
using DoAn4.DTOs.AuthenticationDTOs;

namespace DoAn4.Services.MessageService
{
    public interface IMessageService
    {
        Task SendMessage(string token , Guid recipientId, string content);
        Task<List<MessageDto>> GetMessages(string token ,Guid friendUserId);
        Task<ResultRespone> DeleteMessage(string token ,Guid messageId);
    }
}
