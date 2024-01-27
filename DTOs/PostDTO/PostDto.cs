namespace DoAn4.DTOs.PostDTO
{
    public class PostDto
    {
        public string? Content { get; set; }
        public List<IFormFile>? ImageFiles { get; set; } = null;
        public List<IFormFile>? VideoFiles { get; set; } = null;
        
    }
}
