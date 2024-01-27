namespace DoAn4.Models
{
    public class UserOTP
    {
        public Guid UserOtpID { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
