using DoAn4.Data;
using DoAn4.DTOs;
using DoAn4.Interfaces;
using DoAn4.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn4.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;

        public MessageRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<Message> SendMessage(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<MessageDto>> GetMessages(Guid curUser, Guid friendUserId)
        {
            var conversations = await _context.Conversations
                .Where(c => (c.UserId1 == curUser && c.UserId2 == friendUserId) || (c.UserId1 == friendUserId && c.UserId2 == curUser))
                .ToListAsync();

            var conversationIds = conversations.Select(c => c.ConversationsId).ToList();

            var messages = await _context.Messages
                .Where(m => conversationIds.Contains(m.ConversationsId))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            var messageDtos = messages.Select(m => new MessageDto
            {
                ConversationId = m.ConversationsId,
                MessageId = m.MessageId,
                SenderId = m.SenderId,
                ReciverId = m.RecipientId,
                Content = m.Content,
                CreateAt = m.CreatedAt
            }).ToList();

            return messageDtos;
        }

        public async Task<Message> GetLastMessage(Guid conversationId)
        {
            return await _context.Messages
                .Where(m => m.ConversationsId == conversationId)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Message>> GetLastMessages(List<Guid> conversationIds)
        {
            var lastMessages = await _context.Messages
                .Where(m => conversationIds.Contains(m.ConversationsId))
                .GroupBy(m => m.ConversationsId)
                .Select(g => g.OrderByDescending(m => m.CreatedAt).FirstOrDefault())
                .ToListAsync();

            return lastMessages;
        }

        public async Task<bool> DeleteMessagesByConversationId(Guid conversationId)
        {
            var messages = await _context.Messages
                .Where(m => m.ConversationsId == conversationId)
                .ToListAsync();

            if (messages == null || messages.Count == 0)
            {
                // Không có tin nhắn nào trong cuộc trò chuyện
                return false;
            }

            _context.Messages.RemoveRange(messages);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteMessagesById(Guid messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
            {
                return false; 
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return true; 
        }

        public async Task<Message> GetMessageById(Guid messageId)
        {
            return await _context.Messages.FindAsync(messageId);
        }
    }
}
