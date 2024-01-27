using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn4.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        public string Email { get; set; } = null!;

        public byte[] PasswordHash { get; set; } = new byte[32];

        public byte[] PasswordSalt { get; set; } = new byte[32];

        [MaxLength(100)]
        public string Fullname { get; set; } = null!;

        [MaxLength(1)]
        public int Gender { get; set; }

        public string? Avatar { get; set; }

        public string? CoverPhoto { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Address { get; set; } = null!;

        public string? Bio { get; set; }

        public DateTime CreateAt { get; set; }

        public bool IsEmailVerified { get; set; }

        public ICollection<Notify> Notifys { get; set; }

        [InverseProperty("CurrenUser")]
        public ICollection<Friendship>? UserReqest { get; set; }

        [InverseProperty("FriendUser")]
        public ICollection<Friendship>? UserReply { get; set; }

    }
}
