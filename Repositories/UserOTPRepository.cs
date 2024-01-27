
using DoAn4.Data;
using DoAn4.Interfaces;
using DoAn4.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn4.Repositories
{
    public class UserOTPRepository : IUserOTPRepository
    {
        private readonly DataContext _context;

        public UserOTPRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserOTP> GetUserOTPByEmail(string email)
        {
            return await _context.UserOTPs.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserOTP> AddUserOTP(UserOTP userOTP)
        {
            await _context.UserOTPs.AddAsync(userOTP);
            await _context.SaveChangesAsync();
            return userOTP;
        }

        public async Task<bool> DeleteUserOTP(string email)
        {
            var userOTP = await GetUserOTPByEmail(email);
            if (userOTP != null)
            {
                _context.UserOTPs.Remove(userOTP);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
