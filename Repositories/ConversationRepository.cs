using DoAn4.Data;
using DoAn4.Interfaces;
using DoAn4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace DoAn4.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly DataContext _context;

        public ConversationRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<Conversations> CreateConversation(Guid userId1, Guid userId2)
        {
            var conversation = new Conversations
            {
                ConversationsId = Guid.NewGuid(),
                UserId1 = userId1,
                UserId2 = userId2
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }

        public async Task<List<Conversations>> GetAllConversations(Guid userId)
        {
            return await _context.Conversations
                .Where(c => c.UserId1 == userId || c.UserId2 == userId)
                .ToListAsync();
        }

        public async Task<Conversations> GetConversation(Guid userId1, Guid userId2)
        {
            return await _context.Conversations
                .FirstOrDefaultAsync(c =>
                    (c.UserId1 == userId1 && c.UserId2 == userId2) ||
                    (c.UserId1 == userId2 && c.UserId2 == userId1));
        }
       

        public async Task<Conversations> GetConversationById(Guid conversationId)
        {
            return await _context.Conversations.FirstOrDefaultAsync(c =>(c.ConversationsId == conversationId));
                  
        }

        public async Task<bool> DeleteConversation(Conversations conversation)
        {
            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
