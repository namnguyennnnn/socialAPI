using AutoMapper;
using DoAn4.Data;
using DoAn4.DTOs.FriendShipDto;
using DoAn4.Interfaces;
using DoAn4.Models;

using Microsoft.EntityFrameworkCore;

namespace DoAn4.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public FriendshipRepository(DataContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<FriendshipDto> GetFriendshipById(Guid FrendShipId)
        {
            var friendship = await _context.Friendships!.FindAsync(FrendShipId);
            return _mapper.Map<FriendshipDto>(friendship);
        }

        public async Task<Friendship> GetFriendshipByUserIdAndFriendUserId(Guid sender, Guid receiver)
        {
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => (f.UserId == sender && f.FriendUserId == receiver) || (f.UserId == receiver && f.FriendUserId == sender));
            return friendship;
        }

        public async Task AddFriendshipAsync(Friendship friendship)
        {          
            await _context.Friendships.AddAsync(friendship);
             await _context.SaveChangesAsync(); 
        }

        public async Task<bool> IsFriendshipExist(Guid userId, Guid friendUserId)
        {
            var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendUserId == friendUserId)
            || (f.UserId == friendUserId && f.FriendUserId == userId));

            return friendship != null && friendship.FriendStatus == 1;

        }
        public async Task<bool> IsFriendship(Guid friendShipId)
        {
            var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => (f.FriendshipId == friendShipId));
            return friendship != null && friendship.FriendStatus == 1;
        }

        public async Task<bool> IsFriendshipRequestExit(Guid userId, Guid friendUserId)
        {
            var requestIsExit = await _context.Friendships
              .FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendUserId == friendUserId));
            if(requestIsExit != null) {
                return false;
            }

            return true;
        }

        public async Task AcceptFriendAsync(Guid currentUserId, Guid senderId)
        {
            var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => f.UserId == senderId && f.FriendUserId == currentUserId);
            if (friendship != null)
            {
                friendship.FriendStatus = 1;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<FriendshipDto>> GetAllFriendshipPendingAsync(Guid currentUserId)
        {
            var friendshipPending = await _context.Friendships
                .Where(f => (f.FriendUserId == currentUserId ) && f.FriendStatus == 0)
                .ToListAsync();

            return _mapper.Map<List<FriendshipDto>>(friendshipPending);
        }

        public async Task<bool> DeleteFriendshipAsync(Guid friendshipId)
        {
            var delFriendship =  _context.Friendships.FirstOrDefault(f => f.FriendshipId == friendshipId && f.FriendStatus == 1 || f.FriendStatus == 0);
            if (delFriendship != null)
            {
                _context.Friendships.Remove(delFriendship);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<FriendshipDto>> GetAllFriendshipAsync(Guid currentUserId)
        {
            var friendships = await _context.Friendships
                .Where(f => (f.UserId == currentUserId || f.FriendUserId == currentUserId) && f.FriendStatus == 1)
                .ToListAsync();

            return _mapper.Map<List<FriendshipDto>>(friendships);
        }

        public async Task<List<Guid>> GetAllFriendIdsAsync(Guid currentUserId)
        {
            var friendIds = await _context.Friendships
                .Where(f => (f.CurrenUser.UserId == currentUserId || f.FriendUser.UserId == currentUserId)
                            && f.FriendStatus == 1)
                .Select(f => f.CurrenUser.UserId == currentUserId ? f.FriendUser.UserId : f.CurrenUser.UserId)
                .ToListAsync();

            return friendIds;
        }

        public async Task<List<Guid>> GetAllFriendUseridFromListFriendshipAsync(Guid curUser, List<FriendshipDto> friendships)
        {
            return friendships.Where(f => f.UserId != curUser || f.FriendUserId != curUser)
                              .Select(f => f.UserId != curUser ? f.UserId : f.FriendUserId)
                              .ToList();
        }

        public async Task<List<Guid>> GetAllFriendUseridFromListFriendship(List<FriendshipDto> friendships)
        {
            return friendships.Select(f => f.UserId).ToList();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        
    }
}
