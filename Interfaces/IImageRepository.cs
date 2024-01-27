using DoAn4.DTOs;
using DoAn4.Models;

namespace DoAn4.Interfaces
{
    public interface IImageRepository
    {
        Task<Images> GetImageByIdPostAsync(Guid postId);

        Task<Images> GetImageByLinkAsync(string imageLink);

        Task CreateImageAsync(Images images);

        Task<List<Images>> GetRemovedImagesFromPost(Post post, List<string>? Images);

        Task RemoveImageAsync(string imageLink);

        Task<int> SaveChangeAsync();
    }
}
