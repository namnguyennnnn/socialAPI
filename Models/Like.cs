using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn4.Models
{
    public class Like
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PostId { get; set; }

        public Guid UserId { get; set; }

        [MaxLength (1)]
        public int React { get; set; }    

        [ForeignKey(nameof(PostId))]        
        public Post Post { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }      
    }
}
