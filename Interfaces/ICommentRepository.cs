using DoAn4.Models;

namespace DoAn4.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment> GetCommentById(Guid commentId);
        Task<IEnumerable<Comment>> GetCommentsByPostId(Guid postId);
        Task AddComment(Comment comment);
        Task UpdateComment(Comment comment);
        Task DeleteComment(Guid commentId);
    }
}
