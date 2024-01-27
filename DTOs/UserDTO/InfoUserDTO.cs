using System.ComponentModel.DataAnnotations;

namespace DoAn4.DTOs.UserDTO
{
    public class InfoUserDTO
    {
        public Guid? UserId { get; set; }

        public string Email { get; set; } = null!;

        [MaxLength(100)]
        public string Fullname { get; set; } = null!;

        public int Gender { get; set; }

        public string Avatar { get; set; }

        public string CoverPhoto { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Address { get; set; } 

        public string? Bio { get; set; }

        public DateTime? CreateAt { get; set; }
        
        public Guid SenderFriendResquest { get; set; }

        public Guid ReciverFriendResquest { get; set; }

        public int FriendStatus { get; set; }

        public Guid FriendShipId { get; set; }

    }
}
