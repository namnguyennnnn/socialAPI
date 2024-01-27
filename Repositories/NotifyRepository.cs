using DoAn4.Data;
using DoAn4.Interfaces;
using DoAn4.Models;
using Mailjet.Client.Resources;
using Microsoft.EntityFrameworkCore;

namespace DoAn4.Repositories
{
    public class NotifyRepository : INotifyRepository
    {
        private readonly DataContext _context;
     

        public NotifyRepository(DataContext context)
        {
            _context = context;
            
        }
        public async Task CreateNotifyAsync(Notify notify)
        {
            await _context.Notifies.AddAsync(notify);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteNotifyByFriendShipId(Guid friendShipId)
        {
            var notify = await _context.Notifies.FirstOrDefaultAsync(n => n.FriendShipId == friendShipId && n.NotifyType == "friend_request");
            if (notify != null)
            {
                _context.Notifies.Remove(notify);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteNotifyById(Guid notifyId )
        {
            var notify = _context.Notifies.FirstOrDefault(n => n.NotifyId == notifyId);
            if (notify != null)
            {
                _context.Notifies.Remove(notify);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        public async Task<List<Notify>> GetAllNotifyByIdUser(Guid userId)
        {
            var result = await _context.Notifies.Where(n => n.UserId == userId).ToListAsync();
            return result;
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
