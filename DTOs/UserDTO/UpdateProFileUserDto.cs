using System.ComponentModel.DataAnnotations;

namespace DoAn4.DTOs.UserDTO
{
    public class UpdateProFileUserDto
    {
        
        [MaxLength(100)]
        public string? Fullname { get; set; }

        public DateTime? DateOfBirth { get; set; } = null;

        public string? Address { get; set; }

        public string? Bio { get; set; }

        public IFormFile? Avatar { get; set; }

        public IFormFile? Coverphoto { get; set; }
    }
}
