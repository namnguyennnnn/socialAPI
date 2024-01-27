namespace DoAn4.DTOs.FriendShipDto
{
    public class FriendResultRespone
    {
        public Guid FriendShipId { get; set; }
        public Guid SenderFriendResquest { get; set; }
        public Guid ReciverFriendResquest { get; set; }
        public int FriendStatus { get; set; }
    }
}
