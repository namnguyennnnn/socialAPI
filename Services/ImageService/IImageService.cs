using DoAn4.DTOs.PostDTO;
using DoAn4.Models;

namespace DoAn4.Services.ImageService
{
    public interface IImageService
    {
        Task<List<Images>> UploadImages(Guid postId, List<IFormFile> file);
        Task<bool> RemoveImage(Guid postId,  UpdatePostDto updatePostDto);
        Task<Images> UploadImage(Guid postId, IFormFile file);

    }
}
