using DoAn4.DTOs;
using DoAn4.DTOs.AuthenticationDTOs;
using DoAn4.Models;

namespace DoAn4.Services.CommentService
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetAllCommentAsync(Guid postId);
        Task<CommentDto> CreateCommentAsync(string token , Guid postId, string content);
        Task<ResultRespone> UpdateCommentAsync(string token, Guid commentId, string content);
        Task<ResultRespone> DeleteCommentAsync(string token,Guid postId,Guid commentId);
    }
}
