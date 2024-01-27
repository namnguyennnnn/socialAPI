using DoAn4.Data;
using DoAn4.Interfaces;
using DoAn4.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn4.Repositories
{
    public class CommentRepository: ICommentRepository
    {
        private readonly DataContext _context;

        public CommentRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Comment> GetCommentById(Guid commentId)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostId(Guid postId)
        {
            return await _context.Comments.Where(c => c.PostId == postId).ToListAsync();
        }

        public async Task AddComment(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateComment(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteComment(Guid commentId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
