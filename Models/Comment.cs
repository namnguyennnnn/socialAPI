using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn4.Models
{
    public class Comment
    {
        [Key]
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string Content { get; set; }
        public DateTime CommentTime { get; set;}
        public DateTime UpdateCommentTime { get; set; }
       

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }
    }
}
