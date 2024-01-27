
using DoAn4.Interfaces;
using DoAn4.Services.FileService;
using DoAn4.Models;
using DoAn4.DTOs.PostDTO;
using static System.Net.Mime.MediaTypeNames;

namespace DoAn4.Services.ImageService
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IFileService _fileService;

        public ImageService(IImageRepository imageRepository, IFileService fileService)
        {
            _imageRepository = imageRepository;
            _fileService = fileService;
        }

        public async Task<bool> RemoveImage(Guid postId, UpdatePostDto updatePostDto)
        {
            var removedImageLinks = new List<string>();
            foreach (var removedImageLink in updatePostDto.ImagesLinkRemove)
            {
                var removedImage = await _imageRepository.GetImageByLinkAsync(removedImageLink);
                if (removedImage != null && removedImage.PostId == postId)
                {
                    removedImageLinks.Add(removedImage.ImageLink);
                    await _imageRepository.RemoveImageAsync(removedImageLink);
                }
            }           
            var isRemove = await _fileService.DeleteFiles(removedImageLinks);
            return isRemove;
        }

        public async Task<Images> UploadImage(Guid postId,IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName);
            if (ext == ".jpg" || ext == ".png" || ext == ".jpeg")
            {
                var fileName = await _fileService.SaveFile(file);

                var img = new Images
                {
                    PostId = postId,
                    ImageId = Guid.NewGuid(),
                    ImageLink = fileName
                };

                await _imageRepository.CreateImageAsync(img);
                return img;
            }
            else
            {
                throw new Exception("Cập nhật file ảnh thất bại vui lòng chọn file ảnh đúng định dạng");
            }
            
            
        }

        public async Task<List<Images>> UploadImages(Guid postId, List<IFormFile> files)
        {
            var images = new List<Images>();

            foreach (var file in files)
            {
                var ext = Path.GetExtension(file.FileName);
                if (ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                {
                    var fileName = await _fileService.SaveFile(file);

                    var img = new Images
                    {
                        PostId = postId,
                        ImageId = Guid.NewGuid(),
                        ImageLink = fileName
                    };

                    await _imageRepository.CreateImageAsync(img);
                    images.Add(img);
                }
                else
                {
                    throw new Exception("Cập nhật file ảnh thất bại vui lòng chọn file ảnh đúng định dạng");
                }
            }

            return images;

        }
    }
}
