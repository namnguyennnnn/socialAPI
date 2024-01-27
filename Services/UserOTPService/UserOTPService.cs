using DoAn4.Interfaces;
using DoAn4.Models;
using DoAn4.Services.EmailService;

namespace DoAn4.Services.UserOTPService
{
    public class UserOTPService: IUserOTPService
    {
        private readonly IUserOTPRepository _userOTPRepository;
        private readonly IEmailService _emailService;
        public UserOTPService(IUserOTPRepository userOTPRepository, IEmailService emailService)
        {
            _userOTPRepository = userOTPRepository;
            _emailService = emailService;
        }
        public async Task<string> GenerateOTPAndSendToEmail(string email)
        {
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var otp = new string(Enumerable.Repeat(characters, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            var userOTP = new UserOTP
            {
                UserOtpID = Guid.NewGuid(),
                Email = email,
                Token = otp
            };


            try
            {
                var emailBody = $"<p style=\"font-size: 16px;\">This is your verification code:</p><h2 style=\"font-size: 24px; background-color: #f5f5f5; padding: 10px;\">{otp}</h2>";
                await _emailService.SendEmailAsync(email, "Email Verification", emailBody);

                await _userOTPRepository.AddUserOTP(userOTP);

                return otp;
            }
            catch (Exception ex)
            {         
                throw new ApplicationException("Failed to send verification email.", ex);
            }
        }
    }
}
