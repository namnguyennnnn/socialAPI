using DoAn4.DTOs.UserDTO;

namespace DoAn4.DTOs.PostDTO
{
    public class InFoPostDto
    {
        public Guid PostId { get; set; }        

        public string? Content { get; set; }

        public int TotalReact { get; set; }

        public int TotalComment { get; set; }

        public DateTime PostTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public int React { get; set; }

        public InfoUserDTO User { get; set; }

        public List<string> Images { get; set; }

        public List<string> Videos { get; set; }
    }
}
