using DoAn4.Data;
using DoAn4.Interfaces;
using DoAn4.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn4.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly DataContext _context;

        public LikeRepository(DataContext context)
        {
            _context = context;

        }

        public async Task<Like> GetLike(Guid postId, Guid userId)
        {
            return await _context.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
        }

        public async Task<List<Guid>> GetIdsUserLikedAsync(Guid postId)
        {

            return await _context.Likes
            .Where(like => like.PostId == postId) 
            .Select(like => like.UserId)
            .ToListAsync();
        }

        public async Task CreateLike(Like like)
        {
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLike(Like like)
        {
            _context.Likes.Update(like);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLike(Like like)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        
    }
}
