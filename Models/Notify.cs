
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn4.Models
{
    public class Notify
    {
        public Guid NotifyId { get; set; }

        public string NotifyContent { get; set; }

        public DateTime NotifyTime { get; set; }

        public string NotifyType { get; set; }

        public bool IsRead  { get; set; }
 
        public Guid PostId { get; set; }
   
        public Guid FriendShipId { get; set; }

        public Guid SenderId { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }
   
        

    }
}
