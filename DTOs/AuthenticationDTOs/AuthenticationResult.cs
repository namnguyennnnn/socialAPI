using DoAn4.DTOs.UserDTO;

namespace DoAn4.DTOs.AuthenticationDTOs
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string[]? Errors { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public InfoUserDTO User { get; set; }
    }
}
