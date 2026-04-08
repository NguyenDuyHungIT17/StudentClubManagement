using StudentClub.Domain.Entities.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.Interfaces
{
    public interface IChatRepository
    {
        // ============================================
        // ChatMessage Operations
        // ============================================
        /// <summary>
        /// Lấy tin nhắn theo ID
        /// </summary>
        Task<ChatMessage?> GetMessageByIdAsync(int messageId);

        /// <summary>
        /// Lấy toàn bộ tin nhắn giữa 2 user (tin nhắn riêng)
        /// </summary>
        Task<List<ChatMessage>> GetPrivateMessagesAsync(int user1Id, int user2Id, int pageNumber = 1, int pageSize = 50);

        /// <summary>
        /// Lấy toàn bộ tin nhắn của 1 CLB (tin nhắn nhóm)
        /// </summary>
        Task<List<ChatMessage>> GetGroupMessagesAsync(int clubId, int pageNumber = 1, int pageSize = 50);

        /// <summary>
        /// Lấy tin nhắn từ khách tới leader
        /// </summary>
        Task<List<ChatMessage>> GetGuestMessagesAsync(int clubId, int pageNumber = 1, int pageSize = 50);

        /// <summary>
        /// Thêm tin nhắn mới
        /// </summary>
        Task<int> AddMessageAsync(ChatMessage message);

        /// <summary>
        /// Cập nhật tin nhắn (cập nhật status)
        /// </summary>
        Task UpdateMessageAsync(ChatMessage message);

        /// <summary>
        /// Xóa tin nhắn (soft delete)
        /// </summary>
        Task DeleteMessageAsync(int messageId);

        // ============================================
        // ChatConversation Operations
        // ============================================
        /// <summary>
        /// Lấy cuộc trò chuyện giữa 2 user
        /// </summary>
        Task<ChatConversation?> GetConversationAsync(int user1Id, int user2Id);

        /// <summary>
        /// Lấy danh sách cuộc trò chuyện của user
        /// </summary>
        Task<List<ChatConversation>> GetUserConversationsAsync(int userId, int pageNumber = 1, int pageSize = 10);

        /// <summary>
        /// Tạo hoặc cập nhật cuộc trò chuyện
        /// </summary>
        Task<int> CreateOrUpdateConversationAsync(int user1Id, int user2Id, int lastMessageId, DateTime lastMessageTime);

        /// <summary>
        /// Xóa cuộc trò chuyện
        /// </summary>
        Task DeleteConversationAsync(int conversationId);

        // ============================================
        // ChatUnreadMessage Operations
        // ============================================
        /// <summary>
        /// Lấy số tin nhắn chưa đọc của user
        /// </summary>
        Task<int> GetUnreadCountAsync(int userId);

        /// <summary>
        /// Lấy danh sách tin nhắn chưa đọc của user
        /// </summary>
        Task<List<ChatUnreadMessage>> GetUnreadMessagesAsync(int userId);

        /// <summary>
        /// Thêm tin nhắn chưa đọc
        /// </summary>
        Task AddUnreadMessageAsync(int userId, int messageId);

        /// <summary>
        /// Đánh dấu tin đã đọc
        /// </summary>
        Task MarkAsReadAsync(int unreadId);

        /// <summary>
        /// Đánh dấu tất cả tin từ sender đã đọc
        /// </summary>
        Task MarkAllAsReadAsync(int userId, int senderId);

        /// <summary>
        /// Xóa tin nhắn chưa đọc
        /// </summary>
        Task DeleteUnreadMessageAsync(int unreadId);

        // ============================================
        // Utility Operations
        // ============================================
        /// <summary>
        /// Lưu thay đổi vào database
        /// </summary>
        Task SaveChangeAsync();
    }
}
