using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn4.Models
{
    public class Video
    {
        [Key]
        public Guid VideoId { get; set; }

        public Guid PostId { get; set; }

        public string VideoLink { get; set; }

        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }

    }
}
