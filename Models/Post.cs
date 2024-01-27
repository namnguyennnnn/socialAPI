using AutoMapper.Configuration.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace DoAn4.Models
{
    public class Post
    {
        [Key]
        public Guid PostId { get; set; }

        public Guid UserPostId { get; set; }

        public string? Content { get; set; }

        public int TotalReact { get; set; }

        public int TotalComment { get; set; }

        public DateTime PostTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public bool IsDeleted { get; set; }
     

        [ForeignKey(nameof(UserPostId))]
        public User User { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<Images> Images { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<Video> Videos { get; set; }
    }
}
