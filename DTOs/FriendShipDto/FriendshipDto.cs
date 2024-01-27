namespace DoAn4.DTOs.FriendShipDto
{
    public class FriendshipDto
    {
        public Guid FriendShipId { get; set; }
        public Guid UserId { get; set; }
        public Guid FriendUserId { get; set; }
        public int FriendStatus { get; set; }
    }
}
