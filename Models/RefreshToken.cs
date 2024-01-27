using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoAn4.Models
{
    public class RefreshToken
    {
        [Key]
        public Guid RefreshTokenId { get; set; }

        public string Token { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        public bool IsRevoked { get; set; }

        public Guid UserId { get; set; }

        
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
