using DoAn4.DTOs;
using DoAn4.Models;

namespace DoAn4.Interfaces
{
    public interface IVideoRepository
    {
        Task<Video> GetVideoByIdPostAsync(Guid postId);

        Task CreateVideoAsync(Video video);

        Task<Video> GetVideoByLinkAsync(string videoLink);

        Task RemoveVideoAsync(string videoLink);

        Task<int> SaveChangeAsync();
    }
}
