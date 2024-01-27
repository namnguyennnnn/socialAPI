namespace DoAn4.DTOs.PostDTO
{
    public class UpdatePostDto
    {
        
        public string? Content { get; set; }=null;
        public List<string>? ImagesLinkRemove { get; set; } = null;
        public List<string>? VideosLinkRemove { get; set; } = null;
        public List<IFormFile>? ImageFiles { get; set; } = null;
        public List<IFormFile>? VideoFiles { get; set; } = null;
    }
}
