using DoAn4.DTOs.PostDTO;
using DoAn4.Models;

namespace DoAn4.Services.VideoService
{
    public interface IVideoService
    {
        Task<List<Video>> UploadVideo(Guid postId, List<IFormFile> files);
        Task<bool> RemoveVideo(Guid postId, UpdatePostDto updatePostDto);
        
    }
}
