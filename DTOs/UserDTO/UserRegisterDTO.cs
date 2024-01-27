using System.ComponentModel.DataAnnotations;

namespace DoAn4.DTOs.UserDTO
{
    public class UserRegisterDTO
    {
       
        [EmailAddress]
        public string Email { get; set; } = null!;

       
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [StringLength(100)]
        public string Fullname { get; set; } = null!;


        public int Gender { get; set; }


        public DateTime DateOfBirth { get; set; }

        
        [StringLength(500)]
        public string Address { get; set; } = null!;

        [Required]
        public string Token { get; set; } = null!;
    }
}
