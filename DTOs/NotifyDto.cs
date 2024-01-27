namespace DoAn4.DTOs
{
    public class NotifyDto
    {
        public Guid NotifyId { get; set; }

        public string NotifyContent { get; set; }

        public DateTime NotifyTime { get; set; }

        public string NotifyType { get; set; }

        public bool IsRead { get; set; }

        public Guid PostId { get; set; }

        public Guid FriendShipId { get; set; }

        public Guid SenderId { get; set; }


        public string Avatar { get; set; } 
    }
}
