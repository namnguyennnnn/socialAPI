
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn4.Models
{
    public class Message
    {
        [Key]
        public Guid MessageId { get; set; }
        public Guid ConversationsId { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        
        public Conversations Conversations { get; set; }

        [ForeignKey(nameof(SenderId))]
        public User UserSend { get; set; }

        [ForeignKey(nameof(RecipientId))]
        public User UserRep { get; set; }

    }
}
