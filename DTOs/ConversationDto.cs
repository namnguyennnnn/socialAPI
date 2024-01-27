namespace DoAn4.DTOs
{
    public class ConversationDto
    {
        public Guid ConversationId { get; set; }
        public string Avatar { get; set; }
        public Guid RecipientId { get; set; }
        public string RecipientName { get; set; }
        public string LastMessage { get; set; }
    }
}
