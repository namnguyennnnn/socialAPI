using DoAn4.DTOs.UserDTO;
using DoAn4.Models;

namespace DoAn4.Interfaces
{
    public interface IUserRepository
    {
        Task<User>GetUserByEmailAsync(string email);

        Task<User> GetUserByIdAsync(Guid userId);

        Task<List<InfoUserDTO>> GetListUserAsync(List<Guid> UserIds);

        Task<List<InfoUserDTO>> GetUsersByKeyWord(Guid curUser, string keyword );

        Task CreateUserAsync(User user);

        Task UpdateUserAsync(User user);

        Task<int> SaveChangesAsync();
        
    }

}
