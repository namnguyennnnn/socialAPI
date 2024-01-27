namespace DoAn4.DTOs
{
    public class CommentDto
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public Guid CommentId { get; set; }
        public string Content { get; set; }
        public string Fullname { get; set; }
        public string Avatar { get; set; }
        public DateTime CommentTime { get; set; }   
    }
}

