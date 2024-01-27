
using DoAn4.DTOs.AuthenticationDTOs;
using DoAn4.DTOs.UserDTO;
namespace DoAn4.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> LoginAsync(string email, string password);
        
        Task<AuthenticationResult> RefreshAccesstokenTokenAsync(string refreshToken, string accessToken);

        Task<bool> LogoutAsync(string refreshToken, string accessToken);

        Task<ResultRespone> RegisterAsync(UserRegisterDTO request);

        Task<ReadIdUserFromToken> GetIdUserFromAccessToken(string token);
    }
}
