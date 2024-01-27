
namespace DoAn4.DTOs.AuthenticationDTOs
{
    public class ReadIdUserFromToken
    {
        public Guid UserId { get; set; } 

        public string FullName { get; set; }

        public string Email { get; set; } 

        public string Avatar { get; set; }

        public string CoverPhoto { get; set; }

       
    }
}
