using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentClub.Application.DTOs.Chat;
using StudentClub.Application.IServices;
using System.Security.Claims;

namespace StudentClub.API.Controllers
{
    /// <summary>
    /// Controller cho quản lý chat real-time
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy thông tin user hiện tại từ token
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        /// <summary>
        /// Lấy tin nhắn theo ID
        /// </summary>
        [HttpGet("messages/{messageId}")]
        public async Task<IActionResult> GetMessageById(int messageId)
        {
            try
            {
                var result = await _chatService.GetMessageByIdAsync(messageId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tin nhắn");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy tin nhắn riêng tư giữa 2 user
        /// </summary>
        [HttpPost("messages/private")]
        public async Task<IActionResult> GetPrivateMessages([FromBody] GetPrivateMessagesRequestDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                {
                    return Unauthorized(new { message = "Không thể lấy thông tin user" });
                }

                var result = await _chatService.GetPrivateMessagesAsync(currentUserId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tin nhắn riêng");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy tin nhắn nhóm CLB
        /// </summary>
        [HttpPost("messages/group")]
        public async Task<IActionResult> GetGroupMessages([FromBody] GetGroupMessagesRequestDto request)
        {
            try
            {
                var result = await _chatService.GetGroupMessagesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tin nhắn nhóm");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy tin nhắn từ khách tới leader
        /// </summary>
        [HttpGet("messages/guest/{clubId}")]
        [Authorize(Roles = "admin,leader")]
        public async Task<IActionResult> GetGuestMessages(int clubId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var result = await _chatService.GetGuestMessagesAsync(clubId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tin nhắn từ khách");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Tạo tin nhắn mới
        /// </summary>
        [HttpPost("messages")]
        public async Task<IActionResult> CreateMessage([FromBody] CreateChatMessageRequestDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                {
                    return Unauthorized(new { message = "Không thể lấy thông tin user" });
                }

                var result = await _chatService.CreateMessageAsync(currentUserId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo tin nhắn");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa tin nhắn
        /// </summary>
        [HttpDelete("messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            try
            {
                var result = await _chatService.DeleteMessageAsync(messageId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa tin nhắn");
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============================================
        // ChatConversation Endpoints
        // ============================================
        /// <summary>
        /// Lấy danh sách cuộc trò chuyện của user
        /// </summary>
        [HttpGet("conversations")]
        public async Task<IActionResult> GetUserConversations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                {
                    return Unauthorized(new { message = "Không thể lấy thông tin user" });
                }

                var result = await _chatService.GetUserConversationsAsync(currentUserId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách cuộc trò chuyện");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa cuộc trò chuyện
        /// </summary>
        [HttpDelete("conversations/{conversationId}")]
        public async Task<IActionResult> DeleteConversation(int conversationId)
        {
            try
            {
                var result = await _chatService.DeleteConversationAsync(conversationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa cuộc trò chuyện");
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============================================
        // ChatUnreadMessage Endpoints
        // ============================================
        /// <summary>
        /// Lấy số tin nhắn chưa đọc
        /// </summary>
        [HttpGet("unread/count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                {
                    return Unauthorized(new { message = "Không thể lấy thông tin user" });
                }

                var result = await _chatService.GetUnreadCountAsync(currentUserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy số tin chưa đọc");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Đánh dấu tin nhắn đã đọc
        /// </summary>
        [HttpPut("unread/{unreadId}/read")]
        public async Task<IActionResult> MarkAsRead(int unreadId)
        {
            try
            {
                var result = await _chatService.MarkAsReadAsync(unreadId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đánh dấu tin đã đọc");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Đánh dấu tất cả tin từ sender đã đọc
        /// </summary>
        [HttpPut("unread/read-all/{senderId}")]
        public async Task<IActionResult> MarkAllAsRead(int senderId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                {
                    return Unauthorized(new { message = "Không thể lấy thông tin user" });
                }

                var result = await _chatService.MarkAllAsReadAsync(currentUserId, senderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đánh dấu tất cả tin đã đọc");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}