using DoAn4.Data;
using DoAn4.Interfaces;
using DoAn4.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn4.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DataContext _context;

        public RefreshTokenRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRefreshTokenAsync(string token)
        {
            var delRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
            if (delRefreshToken != null)
            {
                _context.RefreshTokens.Remove(delRefreshToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
