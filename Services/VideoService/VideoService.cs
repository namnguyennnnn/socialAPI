using DoAn4.DTOs.PostDTO;
using DoAn4.Interfaces;
using DoAn4.Models;
using DoAn4.Repositories;
using DoAn4.Services.FileService;
using static System.Net.Mime.MediaTypeNames;

namespace DoAn4.Services.VideoService
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IFileService _fileService;

        public VideoService(IVideoRepository videoRepository, IFileService fileService)
        {
            _videoRepository = videoRepository;
            _fileService = fileService;
        }
        public async Task<List<Video>> UploadVideo(Guid postId, List<IFormFile> files)
        {
            var videos = new List<Video>();

            foreach (var file in files)
            {
                var ext = Path.GetExtension(file.FileName);
                if (ext == ".mp4" || ext == ".avi" || ext == ".mov")
                {
                    var fileName = await _fileService.SaveFile(file);

                    var video = new Video
                    {
                        PostId = postId,
                        VideoId = Guid.NewGuid(),
                        VideoLink = fileName
                    };
                    await _videoRepository.CreateVideoAsync(video);
                    videos.Add(video);
                }
                else
                {
                    throw new Exception("Cập nhật file video thất bại vui lòng chọn file video đúng định dạng");
                }
            }

            return videos;
        }
        public async Task<bool> RemoveVideo(Guid postId, UpdatePostDto updatePostDto)
        {
            var removedVideoLinks = new List<string>();
            foreach (var removedVideoLink in updatePostDto.VideosLinkRemove)
            {
                var removedVideo = await _videoRepository.GetVideoByLinkAsync(removedVideoLink);
                if (removedVideo != null && removedVideo.PostId == postId)
                {
                    removedVideoLinks.Add(removedVideo.VideoLink);
                    await _videoRepository.RemoveVideoAsync(removedVideoLink);
                }
            }
            // Remove the images from storage
            var isRemove = await _fileService.DeleteFiles(removedVideoLinks);
            return isRemove;
        }
    }
}
