using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.Chat;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;
using StudentClub.Domain.Entities.Realtime;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ChatService> _logger;

        public ChatService(IChatRepository chatRepository,
                IClubRepository clubRepository,
                IUserRepository userRepository,
            ILogger<ChatService> logger)
        {
            _chatRepository = chatRepository;
            _clubRepository = clubRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        // ============================================
        // ChatMessage Operations
        // ============================================
        public async Task<ApiResponse<ChatMessageResponseDto>> GetMessageByIdAsync(int messageId)
        {
            try
            {
                var message = await _chatRepository.GetMessageByIdAsync(messageId);
                if (message == null)
                {
                    return ApiResponse<ChatMessageResponseDto>.Failure(404, "Tin nhắn không tồn tại");
                }

                var result = await BuildMessageDto(message);

                return ApiResponse<ChatMessageResponseDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tin nhắn. MessageId: {MessageId}", messageId);
                return ApiResponse<ChatMessageResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<PagedResponse<ChatMessageResponseDto>>> GetPrivateMessagesAsync(
    int currentUserId,
    GetPrivateMessagesRequestDto request)
        {
            try
            {
                if (request.UserId == currentUserId)
                {
                    return ApiResponse<PagedResponse<ChatMessageResponseDto>>
                        .Failure(400, "Không thể chat với chính mình");
                }

                var messages = await _chatRepository.GetPrivateMessagesAsync(
                    currentUserId,
                    request.UserId,
                    request.PageNumber,
                    request.PageSize);

                var dtos = new List<ChatMessageResponseDto>();

                foreach (var message in messages)
                {
                    dtos.Add(await BuildMessageDto(message));
                }

                return ApiResponse<PagedResponse<ChatMessageResponseDto>>.Success(
                    new PagedResponse<ChatMessageResponseDto>
                    {
                        Items = dtos,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalCount = messages.Count,
                        TotalPages = (int)Math.Ceiling(messages.Count / (double)request.PageSize)
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tin nhắn riêng");
                return ApiResponse<PagedResponse<ChatMessageResponseDto>>.Failure(500, ex.Message);
            }
        }
        public async Task<ApiResponse<PagedResponse<ChatMessageResponseDto>>> GetGroupMessagesAsync(
            GetGroupMessagesRequestDto request)
        {
            try
            {
                var messages = await _chatRepository.GetGroupMessagesAsync(
                    request.ClubId,
                    request.PageNumber,
                    request.PageSize);

                var dtos = new List<ChatMessageResponseDto>();

                foreach (var message in messages)
                {
                    dtos.Add(await BuildMessageDto(message));
                }

                var totalCount = messages.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

                var pagedResponse = new PagedResponse<ChatMessageResponseDto>
                {
                    Items = dtos,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount
                };

                return ApiResponse<PagedResponse<ChatMessageResponseDto>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tin nhắn nhóm. ClubId: {ClubId}", request.ClubId);
                return ApiResponse<PagedResponse<ChatMessageResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<PagedResponse<ChatMessageResponseDto>>> GetGuestMessagesAsync(
    int clubId,
    int pageNumber = 1,
    int pageSize = 50)
        {
            try
            {
                var messages = await _chatRepository.GetGuestMessagesAsync(clubId, pageNumber, pageSize);

                var dtos = new List<ChatMessageResponseDto>();

                foreach (var message in messages)
                {
                    dtos.Add(await BuildMessageDto(message));
                }

                var totalCount = messages.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var pagedResponse = new PagedResponse<ChatMessageResponseDto>
                {
                    Items = dtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount
                };

                return ApiResponse<PagedResponse<ChatMessageResponseDto>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tin nhắn từ khách. ClubId: {ClubId}", clubId);
                return ApiResponse<PagedResponse<ChatMessageResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<ChatMessageResponseDto>> CreateMessageAsync(
    int senderId,
    CreateChatMessageRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    return ApiResponse<ChatMessageResponseDto>
                        .Failure(400, "Nội dung không được để trống");
                }

                var message = ChatMapper.ToEntity(request, senderId);
                var messageId = await _chatRepository.AddMessageAsync(message);

                message.MessageId = messageId;

                var result = await BuildMessageDto(message);

                return ApiResponse<ChatMessageResponseDto>.Success(result, "Gửi thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo tin nhắn");
                return ApiResponse<ChatMessageResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteMessageAsync(int messageId)
        {
            try
            {
                var message = await _chatRepository.GetMessageByIdAsync(messageId);
                if (message == null)
                {
                    return ApiResponse.Failure(404, "Tin nhắn không tồn tại");
                }

                await _chatRepository.DeleteMessageAsync(messageId);
                return ApiResponse.Success("Xóa tin nhắn thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa tin nhắn. MessageId: {MessageId}", messageId);
                return ApiResponse.Failure(500, ex.Message);
            }
        }

        // ============================================
        // ChatConversation Operations
        // ============================================
        public async Task<ApiResponse<PagedResponse<ChatConversationResponseDto>>> GetUserConversationsAsync(
            int userId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var conversations = await _chatRepository.GetUserConversationsAsync(userId, pageNumber, pageSize);

                var dtos = ChatMapper.ToChatConversationDtoList(conversations);

                var totalCount = conversations.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var pagedResponse = new PagedResponse<ChatConversationResponseDto>
                {
                    Items = dtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount
                };

                return ApiResponse<PagedResponse<ChatConversationResponseDto>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách cuộc trò chuyện. UserId: {UserId}", userId);
                return ApiResponse<PagedResponse<ChatConversationResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteConversationAsync(int conversationId)
        {
            try
            {
                await _chatRepository.DeleteConversationAsync(conversationId);
                return ApiResponse.Success("Xóa cuộc trò chuyện thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa cuộc trò chuyện. ConversationId: {ConversationId}", conversationId);
                return ApiResponse.Failure(500, ex.Message);
            }
        }

        // ============================================
        // ChatUnreadMessage Operations
        // ============================================
        public async Task<ApiResponse<UnreadCountResponseDto>> GetUnreadCountAsync(int userId)
        {
            try
            {
                var unreadCount = await _chatRepository.GetUnreadCountAsync(userId);
                var result = ChatMapper.ToUnreadCountDto(unreadCount);

                return ApiResponse<UnreadCountResponseDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy số tin chưa đọc. UserId: {UserId}", userId);
                return ApiResponse<UnreadCountResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse> MarkAsReadAsync(int unreadId)
        {
            try
            {
                await _chatRepository.MarkAsReadAsync(unreadId);
                return ApiResponse.Success("Đánh dấu tin đã đọc thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đánh dấu tin đã đọc. UnreadId: {UnreadId}", unreadId);
                return ApiResponse.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse> MarkAllAsReadAsync(int userId, int senderId)
        {
            try
            {
                await _chatRepository.MarkAllAsReadAsync(userId, senderId);
                return ApiResponse.Success("Đánh dấu tất cả tin đã đọc thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đánh dấu tất cả tin đã đọc. UserId: {UserId}, SenderId: {SenderId}", userId, senderId);
                return ApiResponse.Failure(500, ex.Message);
            }
        }
        private async Task<ChatMessageResponseDto> BuildMessageDto(ChatMessage message)
        {
            var fromUser = await _userRepository.GetUserNameByIdAsync(message.SenderId);

            var toUser = message.RecipientId.HasValue
                ? await _userRepository.GetUserNameByIdAsync(message.RecipientId.Value)
                : null;

            var clubName = message.ClubId.HasValue
                ? await _clubRepository.GetCLubNameByClubIdAsync(message.ClubId.Value)
                : null;

            return ChatMapper.ToChatMessageDto(message, fromUser, toUser, clubName);
        }
    }


}