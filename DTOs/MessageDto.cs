namespace DoAn4.DTOs
{
    public class MessageDto
    {
        public Guid ConversationId { get; set; }
        public Guid MessageId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReciverId { get; set; }
        public string Content { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
