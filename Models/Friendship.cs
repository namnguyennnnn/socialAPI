
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn4.Models
{
    public class Friendship
    {
        [Key]
        public Guid FriendshipId { get; set; }

        public Guid UserId { get; set; }

        public Guid FriendUserId { get; set; }

        public int FriendStatus { get; set; }

        [ForeignKey("UserId")]
        public User CurrenUser { get; set; }

        [ForeignKey("FriendUserId")]
        public User FriendUser { get; set; }
        
    }
}
