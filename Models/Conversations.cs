using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn4.Models
{
    public class Conversations
    {
        [Key]
        public Guid ConversationsId { get; set; }
        public Guid UserId1 { get; set; }
        public Guid UserId2 { get; set; }

        [ForeignKey(nameof(UserId1))]
        public User User1 { get; set; }

        [ForeignKey(nameof(UserId2))]
        public User User2 { get; set; }

        public ICollection<Message> Messages { get; set; }

    }
}
