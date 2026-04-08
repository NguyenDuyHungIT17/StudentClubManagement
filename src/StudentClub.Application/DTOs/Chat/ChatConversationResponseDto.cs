using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.Chat
{
    /// <summary>
    /// Response DTO cho cuộc trò chuyện
    /// </summary>
    public class ChatConversationResponseDto
    {
        /// <summary>
        /// ID cuộc trò chuyện
        /// </summary>
        public int ConversationId { get; set; }

        /// <summary>
        /// ID user 1
        /// </summary>
        public int User1Id { get; set; }

        /// <summary>
        /// Tên user 1
        /// </summary>
        public string? User1Name { get; set; }

        /// <summary>
        /// ID user 2
        /// </summary>
        public int User2Id { get; set; }

        /// <summary>
        /// Tên user 2
        /// </summary>
        public string? User2Name { get; set; }

        /// <summary>
        /// Tin nhắn cuối cùng
        /// </summary>
        public string? LastMessageContent { get; set; }

        /// <summary>
        /// Thời gian tin nhắn cuối cùng
        /// </summary>
        public DateTime? LastMessageTime { get; set; }

        /// <summary>
        /// Cuộc trò chuyện còn active không
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Thời gian cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}