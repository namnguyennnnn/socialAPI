using DoAn4.DTOs;
using DoAn4.DTOs.PostDTO;
using DoAn4.Models;

namespace DoAn4.Interfaces
{
    public interface IPostRepository
    {
        Task<List<InFoPostDto>> GetNewFeeds(List<Guid> friendId,Guid curentUserId, int skip, int take);

        Task<Guid> CreatePostAsync(Post post);

        Task<List<InFoPostDto>> GetAllPostByIdUserAsync(Guid userId);

        Task<Post> GetPostByIdAsync(Guid postId);
        
        Task<InFoPostDto> GetInfoPostByIdAsync(Guid postId);
        Task UpdatePostAsync(Post post);

        Task<bool> DeletePostAsync(Guid PostId);

        Task<int> SaveChangesAsync();
    }
}
