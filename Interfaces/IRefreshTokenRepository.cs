using DoAn4.Models;

namespace DoAn4.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);

        Task<RefreshToken> GetRefreshTokenAsync(string token);

        Task AddRefreshTokenAsync(RefreshToken refreshToken);

        Task DeleteRefreshTokenAsync(string token);

        Task<int> SaveChangesAsync();
    }
}
