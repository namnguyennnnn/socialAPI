namespace DoAn4.Services.UserOTPService
{
    public interface IUserOTPService
    {
        Task<string> GenerateOTPAndSendToEmail(string email);
    }
}
