using StudentClub.Application.DTOs.Chat;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.IServices
{
    /// <summary>
    /// Service interface cho quản lý chat
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Lấy tin nhắn theo ID
        /// </summary>
        Task<ApiResponse<ChatMessageResponseDto>> GetMessageByIdAsync(int messageId);

        /// <summary>
        /// Lấy tin nhắn riêng tư giữa 2 user
        /// </summary>
        Task<ApiResponse<PagedResponse<ChatMessageResponseDto>>> GetPrivateMessagesAsync(int currentUserId, GetPrivateMessagesRequestDto request);

        /// <summary>
        /// Lấy tin nhắn nhóm CLB
        /// </summary>
        Task<ApiResponse<PagedResponse<ChatMessageResponseDto>>> GetGroupMessagesAsync(GetGroupMessagesRequestDto request);

        /// <summary>
        /// Lấy tin nhắn từ khách tới leader
        /// </summary>
        Task<ApiResponse<PagedResponse<ChatMessageResponseDto>>> GetGuestMessagesAsync(
            int clubId,
            int pageNumber = 1,
            int pageSize = 50);

        /// <summary>
        /// Tạo tin nhắn mới
        /// </summary>
        Task<ApiResponse<ChatMessageResponseDto>> CreateMessageAsync(
            int senderId,
            CreateChatMessageRequestDto request);

        /// <summary>
        /// Xóa tin nhắn (soft delete)
        /// </summary>
        Task<ApiResponse> DeleteMessageAsync(int messageId);

        /// <summary>
        /// Lấy danh sách cuộc trò chuyện của user
        /// </summary>
        Task<ApiResponse<PagedResponse<ChatConversationResponseDto>>> GetUserConversationsAsync(
            int userId,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// Xóa cuộc trò chuyện
        /// </summary>
        Task<ApiResponse> DeleteConversationAsync(int conversationId);

        /// <summary>
        /// Lấy số tin nhắn chưa đọc của user
        /// </summary>
        Task<ApiResponse<UnreadCountResponseDto>> GetUnreadCountAsync(int userId);

        /// <summary>
        /// Đánh dấu tin nhắn đã đọc
        /// </summary>
        Task<ApiResponse> MarkAsReadAsync(int unreadId);

        /// <summary>
        /// Đánh dấu tất cả tin từ sender đã đọc
        /// </summary>
        Task<ApiResponse> MarkAllAsReadAsync(int userId, int senderId);
    }
}