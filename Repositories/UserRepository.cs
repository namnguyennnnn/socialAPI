using AutoMapper;
using DoAn4.Data;
using DoAn4.DTOs.UserDTO;
using DoAn4.Interfaces;
using DoAn4.Models;

using Microsoft.EntityFrameworkCore;

namespace DoAn4.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
       

        public UserRepository(DataContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
            
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user =  await _context.Users.FirstOrDefaultAsync(u => u.Email == email);          
            return user;
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
           
            return user;
        }

        
        public async Task<List<InfoUserDTO>> GetListUserAsync(List<Guid> UserIds)
        {
            var users = await _context.Users.Where(u => UserIds.Contains(u.UserId)).ToListAsync();
            var result = _mapper.Map<List<InfoUserDTO>>(users);           
            return result;
        }
        

        public async Task UpdateUserAsync(User user)
        {     
            _context.Users.Update(user);
            await _context.SaveChangesAsync();                 
        }
       

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<List<InfoUserDTO>> GetUsersByKeyWord(Guid curUser,string keyword)
        {
            var users = await _context.Users
                .Where(u => EF.Functions.Like(u.Fullname, $"%{keyword}%") || EF.Functions.Like(u.Email, $"%{keyword}%"))
                .ToListAsync();

            if (users.Count == 0)
            {
                users = await _context.Users
                    .Where(u => u.Fullname.Contains(keyword) || u.Email.Contains(keyword))
                    .ToListAsync();
            }

            var result = new List<InfoUserDTO>();

            foreach (var user in users)
            {
                var infoUser = new InfoUserDTO
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Fullname = user.Fullname,
                    Gender = user.Gender,
                    Avatar = user.Avatar,
                    CoverPhoto = user.CoverPhoto,
                    DateOfBirth = user.DateOfBirth,
                    Address = user.Address,
                    Bio = user.Bio,
                    CreateAt = user.CreateAt
                };

                // Tìm Friendship tương ứng với user
                var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                    (f.UserId == user.UserId && f.FriendUserId == curUser) ||
                    (f.FriendUserId == user.UserId && f.UserId == curUser));

                if (friendship != null)
                {
                    infoUser.SenderFriendResquest = friendship.UserId;
                    infoUser.ReciverFriendResquest = friendship.FriendUserId;
                    infoUser.FriendShipId = friendship.FriendshipId;
                    infoUser.FriendStatus = friendship.FriendStatus;
                }
                else
                {
                    infoUser.FriendStatus = 2;
                }

                result.Add(infoUser);
            }

            return result;
        }

    }
}
