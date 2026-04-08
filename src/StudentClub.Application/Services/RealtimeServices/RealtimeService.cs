using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.DtoRealTime;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices.IRealtimeService;
using StudentClub.Application.Realtime;
using StudentClub.Domain.Enums;

namespace StudentClub.Application.Services.RealtimeServices
{
    /// <summary>
    /// Service xử lý real-time chat qua WebSocket
    /// </summary>
    public class RealtimeService : IRealtimeService
    {
        private readonly IRealtimeConnectionManager _connectionManager;
        private readonly IChatRepository _chatRepository;
        private readonly ILogger<RealtimeService> _logger;

        public RealtimeService(
            IRealtimeConnectionManager connectionManager,
            IChatRepository chatRepository,
            ILogger<RealtimeService> logger)
        {
            _connectionManager = connectionManager;
            _chatRepository = chatRepository;
            _logger = logger;
        }

        /// <summary>
        /// Xử lý tin nhắn từ WebSocket
        /// </summary>
        public async Task HandleAsync(ChatCommand cmd, RealtimeUserContext user)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(cmd.Content))
                {
                    _logger.LogWarning("Empty message from user {UserId}", user.UserId);
                    return;
                }

                // Xử lý theo loại tin nhắn
                switch (cmd.Type)
                {
                    case ChatMessageType.GroupMessage:
                        await HandleGroupMessageAsync(cmd, user);
                        break;

                    case ChatMessageType.PrivateMessage:
                        await HandlePrivateMessageAsync(cmd, user);
                        break;

                    case ChatMessageType.GuestMessage:
                        await HandleGuestMessageAsync(cmd, user);
                        break;

                    default:
                        _logger.LogWarning("Unknown message type: {Type}", cmd.Type);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling chat command from user {UserId}", user.UserId);
            }
        }

        /// <summary>
        /// Xử lý tin nhắn nhóm CLB
        /// </summary>
        private async Task HandleGroupMessageAsync(ChatCommand cmd, RealtimeUserContext user)
        {
            // Kiểm tra user có quyền gửi tin nhắn trong CLB không
            if (!user.ClubId.HasValue || user.ClubId != cmd.ClubId)
            {
                _logger.LogWarning("User {UserId} không có quyền gửi tin nhắn cho CLB {ClubId}",
                    user.UserId, cmd.ClubId);
                return;
            }

            // Lưu tin nhắn vào database
            var chatMessage = new Domain.Entities.Realtime.ChatMessage
            {
                SenderId = user.UserId,
                MessageType = ChatMessageType.GroupMessage,
                Status = ChatMessageStatus.Sent,
                Content = cmd.Content,
                ClubId = cmd.ClubId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            int messageId = await _chatRepository.AddMessageAsync(chatMessage);

            // Lấy sender name từ connection
            var senderConnection = _connectionManager.GetByUser(user.UserId);
            var senderName = senderConnection?.UserName ?? $"User {user.UserId}";

            // Tạo payload để gửi
            var payload = new
            {
                messageId,
                fromUserId = user.UserId,
                senderName,
                type = ChatMessageType.GroupMessage.ToString(),
                status = ChatMessageStatus.Sent.ToString(),
                content = cmd.Content,
                clubId = cmd.ClubId,
                timestamp = DateTime.UtcNow
            };

            // Lấy tất cả connection trong CLB
            var clubConnections = _connectionManager.GetByClub(cmd.ClubId.Value);

            // Gửi broadcast tới tất cả members
            await BroadcastAsync(payload, clubConnections);

            _logger.LogInformation("Group message sent to CLB {ClubId} by user {UserId}",
                cmd.ClubId, user.UserId);
        }

        /// <summary>
        /// Xử lý tin nhắn riêng tư
        /// </summary>
        private async Task HandlePrivateMessageAsync(ChatCommand cmd, RealtimeUserContext user)
        {
            // Validate
            if (!cmd.ToUserId.HasValue)
            {
                _logger.LogWarning("Private message missing ToUserId");
                return;
            }

            if (cmd.ToUserId == user.UserId)
            {
                _logger.LogWarning("User {UserId} trying to message themselves", user.UserId);
                return;
            }

            // Lưu tin nhắn vào database
            var chatMessage = new Domain.Entities.Realtime.ChatMessage
            {
                SenderId = user.UserId,
                RecipientId = cmd.ToUserId.Value,
                MessageType = ChatMessageType.PrivateMessage,
                Status = ChatMessageStatus.Sent,
                Content = cmd.Content,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            int messageId = await _chatRepository.AddMessageAsync(chatMessage);

            // Update hoặc tạo conversation
            await _chatRepository.CreateOrUpdateConversationAsync(
                user.UserId,
                cmd.ToUserId.Value,
                messageId,
                DateTime.UtcNow);

            // Lấy sender name
            var senderConnection = _connectionManager.GetByUser(user.UserId);
            var senderName = senderConnection?.UserName ?? $"User {user.UserId}";

            // Tạo payload
            var payload = new
            {
                messageId,
                fromUserId = user.UserId,
                senderName,
                type = ChatMessageType.PrivateMessage.ToString(),
                status = ChatMessageStatus.Delivered.ToString(),
                content = cmd.Content,
                timestamp = DateTime.UtcNow
            };

            // Lấy connection của người nhận
            var recipientConnection = _connectionManager.GetByUser(cmd.ToUserId.Value);

            // Nếu người nhận online, gửi ngay lập tức
            if (recipientConnection != null)
            {
                await _connectionManager.SendAsync(recipientConnection, payload);
                _logger.LogInformation("Private message sent from user {FromId} to user {ToId}",
                    user.UserId, cmd.ToUserId);
            }
            else
            {
                // Nếu offline, lưu vào ChatUnreadMessages
                await _chatRepository.AddUnreadMessageAsync(cmd.ToUserId.Value, messageId);
                _logger.LogInformation("User {ToId} is offline. Message saved as unread",
                    cmd.ToUserId);
            }
        }

        /// <summary>
        /// Xử lý tin nhắn từ khách tới leader
        /// </summary>
        private async Task HandleGuestMessageAsync(ChatCommand cmd, RealtimeUserContext user)
        {
            // Validate
            if (!cmd.ClubId.HasValue)
            {
                _logger.LogWarning("Guest message missing ClubId");
                return;
            }

            // Lưu tin nhắn vào database
            var chatMessage = new Domain.Entities.Realtime.ChatMessage
            {
                SenderId = user.UserId,
                MessageType = ChatMessageType.GuestMessage,
                Status = ChatMessageStatus.Sent,
                Content = cmd.Content,
                ClubId = cmd.ClubId.Value,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _chatRepository.AddMessageAsync(chatMessage);

            // Lấy sender name
            var senderConnection = _connectionManager.GetByUser(user.UserId);
            var senderName = senderConnection?.UserName ?? $"Guest";

            // Tạo payload
            var payload = new
            {
                fromUserId = user.UserId,
                senderName,
                type = ChatMessageType.GuestMessage.ToString(),
                status = ChatMessageStatus.Sent.ToString(),
                content = cmd.Content,
                clubId = cmd.ClubId,
                timestamp = DateTime.UtcNow
            };

            // Lấy tất cả leader/admin của CLB
            var leaderConnections = _connectionManager.GetLeaders(cmd.ClubId.Value);

            // Gửi tới leaders
            if (leaderConnections.Any())
            {
                await BroadcastAsync(payload, leaderConnections);
                _logger.LogInformation("Guest message sent to leaders of CLB {ClubId}", cmd.ClubId);
            }
        }

        /// <summary>
        /// Gửi broadcast tin nhắn tới nhiều connections
        /// </summary>
        public async Task BroadcastAsync(object payload, List<WebSocketConnection> connections)
        {
            if (connections.Count == 0)
                return;

            var tasks = connections.Select(c => _connectionManager.SendAsync(c, payload)).ToList();
            await Task.WhenAll(tasks);
        }
    }
}