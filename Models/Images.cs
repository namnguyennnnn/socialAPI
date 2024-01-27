using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn4.Models
{
    public class Images
    {
        [Key]
        public Guid ImageId { get; set; }

        public Guid PostId { get; set; }

        public string ImageLink { get; set; }


        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }
    }
}
