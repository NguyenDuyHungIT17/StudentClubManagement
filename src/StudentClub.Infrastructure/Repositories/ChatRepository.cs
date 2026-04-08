using Microsoft.EntityFrameworkCore;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities.Realtime;
using StudentClub.Infrastructure.Persistence;

namespace StudentClub.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly StudentClubDbContext _context;

        public ChatRepository(StudentClubDbContext context)
        {
            _context = context;
        }

        // ============================================
        // ChatMessage Operations
        // ============================================
        public async Task<ChatMessage?> GetMessageByIdAsync(int messageId)
        {
            return await _context.ChatMessages
                .FirstOrDefaultAsync(m => m.MessageId == messageId && !m.IsDeleted);
        }

        public async Task<List<ChatMessage>> GetPrivateMessagesAsync(int user1Id, int user2Id, int pageNumber = 1, int pageSize = 50)
        {
            return await _context.ChatMessages
                .Where(m =>
                    !m.IsDeleted &&
                    m.ClubId == null &&
                    ((m.SenderId == user1Id && m.RecipientId == user2Id) ||
                     (m.SenderId == user2Id && m.RecipientId == user1Id)))
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<ChatMessage>> GetGroupMessagesAsync(int clubId, int pageNumber = 1, int pageSize = 50)
        {
            return await _context.ChatMessages
                .Where(m =>
                    !m.IsDeleted &&
                    m.ClubId == clubId &&
                    m.RecipientId == null)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<ChatMessage>> GetGuestMessagesAsync(int clubId, int pageNumber = 1, int pageSize = 50)
        {
            return await _context.ChatMessages
                .Where(m =>
                    !m.IsDeleted &&
                    m.ClubId == clubId &&
                    m.RecipientId != null)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> AddMessageAsync(ChatMessage message)
        {
            await _context.ChatMessages.AddAsync(message);
            await SaveChangeAsync();
            return message.MessageId;
        }

        public async Task UpdateMessageAsync(ChatMessage message)
        {
            _context.ChatMessages.Update(message);
            await SaveChangeAsync();
        }

        public async Task DeleteMessageAsync(int messageId)
        {
            var message = await GetMessageByIdAsync(messageId);
            if (message != null)
            {
                message.IsDeleted = true;
                message.DeletedAt = DateTime.UtcNow;
                await UpdateMessageAsync(message);
            }
        }

        // ============================================
        // ChatConversation Operations
        // ============================================
        public async Task<ChatConversation?> GetConversationAsync(int user1Id, int user2Id)
        {
            var minId = Math.Min(user1Id, user2Id);
            var maxId = Math.Max(user1Id, user2Id);

            return await _context.ChatConversations
                .FirstOrDefaultAsync(c => c.User1Id == minId && c.User2Id == maxId && c.IsActive);
        }

        public async Task<List<ChatConversation>> GetUserConversationsAsync(int userId, int pageNumber = 1, int pageSize = 10)
        {
            return await _context.ChatConversations
                .Where(c => (c.User1Id == userId || c.User2Id == userId) && c.IsActive)
                .OrderByDescending(c => c.LastMessageTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CreateOrUpdateConversationAsync(int user1Id, int user2Id, int lastMessageId, DateTime lastMessageTime)
        {
            var minId = Math.Min(user1Id, user2Id);
            var maxId = Math.Max(user1Id, user2Id);

            var conversation = await _context.ChatConversations
                .FirstOrDefaultAsync(c => c.User1Id == minId && c.User2Id == maxId);

            if (conversation == null)
            {
                conversation = new ChatConversation
                {
                    User1Id = minId,
                    User2Id = maxId,
                    LastMessageId = lastMessageId,
                    LastMessageTime = lastMessageTime,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.ChatConversations.AddAsync(conversation);
            }
            else
            {
                conversation.LastMessageId = lastMessageId;
                conversation.LastMessageTime = lastMessageTime;
                conversation.UpdatedAt = DateTime.UtcNow;
                _context.ChatConversations.Update(conversation);
            }

            await SaveChangeAsync();
            return conversation.ConversationId;
        }

        public async Task DeleteConversationAsync(int conversationId)
        {
            var conversation = await _context.ChatConversations.FindAsync(conversationId);
            if (conversation != null)
            {
                conversation.IsActive = false;
                _context.ChatConversations.Update(conversation);
                await SaveChangeAsync();
            }
        }

        // ============================================
        // ChatUnreadMessage Operations
        // ============================================
        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.ChatUnreadMessages
                .CountAsync(u => u.UserId == userId && !u.IsRead);
        }

        public async Task<List<ChatUnreadMessage>> GetUnreadMessagesAsync(int userId)
        {
            return await _context.ChatUnreadMessages
                .Where(u => u.UserId == userId && !u.IsRead)
                .OrderByDescending(u => u.ReceivedAt)
                .ToListAsync();
        }

        public async Task AddUnreadMessageAsync(int userId, int messageId)
        {
            var existingUnread = await _context.ChatUnreadMessages
                .FirstOrDefaultAsync(u => u.UserId == userId && u.MessageId == messageId);

            if (existingUnread == null)
            {
                var unreadMessage = new ChatUnreadMessage
                {
                    UserId = userId,
                    MessageId = messageId,
                    ReceivedAt = DateTime.UtcNow,
                    IsRead = false
                };
                await _context.ChatUnreadMessages.AddAsync(unreadMessage);
                await SaveChangeAsync();
            }
        }

        public async Task MarkAsReadAsync(int unreadId)
        {
            var unread = await _context.ChatUnreadMessages.FindAsync(unreadId);
            if (unread != null)
            {
                unread.IsRead = true;
                unread.ReadAt = DateTime.UtcNow;
                _context.ChatUnreadMessages.Update(unread);
                await SaveChangeAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int userId, int senderId)
        {
            var unreadMessages = await _context.ChatUnreadMessages
                .Where(u => u.UserId == userId && !u.IsRead)
                .ToListAsync();

            // Filter in-memory vì không có FK relationship
            var filteredMessages = unreadMessages
                .Where(u =>
                {
                    var message = _context.ChatMessages.FirstOrDefault(m => m.MessageId == u.MessageId);
                    return message != null && message.SenderId == senderId;
                })
                .ToList();

            foreach (var unread in filteredMessages)
            {
                unread.IsRead = true;
                unread.ReadAt = DateTime.UtcNow;
            }

            if (filteredMessages.Any())
            {
                _context.ChatUnreadMessages.UpdateRange(filteredMessages);
                await SaveChangeAsync();
            }
        }

        public async Task DeleteUnreadMessageAsync(int unreadId)
        {
            var unread = await _context.ChatUnreadMessages.FindAsync(unreadId);
            if (unread != null)
            {
                _context.ChatUnreadMessages.Remove(unread);
                await SaveChangeAsync();
            }
        }

        // ============================================
        // Utility Operations
        // ============================================
        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}