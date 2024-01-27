
using Microsoft.Extensions.Options;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using DoAn4.Helper;

namespace DoAn4.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var client = new MailjetClient(_emailSettings.ApiKey, _emailSettings.ApiSecret);
            var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(_emailSettings.SenderEmail,"Nam nguyễn"))
                .WithTo(new SendContact(toEmail))
                .WithSubject(subject)
                .WithHtmlPart(message)
                .Build();
           
            var response = await client.SendTransactionalEmailAsync(email);

            if (response.Messages.Length == 0 || response.Messages[0].Status != "success")
            {
                throw new ApplicationException($"Error sending email. Status: {response.Messages[0].Status}");
            }
        }
    } 
}
