
namespace DoAn4.Services.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }

}
