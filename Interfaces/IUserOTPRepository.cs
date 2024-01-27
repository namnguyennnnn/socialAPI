using DoAn4.Models;

namespace DoAn4.Interfaces
{
    public interface IUserOTPRepository
    {
        Task<UserOTP> GetUserOTPByEmail(string email);
        Task<UserOTP> AddUserOTP(UserOTP userOTP);
        Task<bool> DeleteUserOTP(string email);
    }
}
