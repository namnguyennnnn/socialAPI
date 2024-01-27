using DoAn4.Models;

namespace DoAn4.Interfaces
{
    public interface ILikeRepository
    {
        Task<Like> GetLike(Guid postId, Guid userId);

        Task<List<Guid>> GetIdsUserLikedAsync(Guid postId);

        Task CreateLike(Like like);

        Task UpdateLike(Like like);

        Task DeleteLike(Like like);

        Task<int> SaveChangesAsync();
    }
}
