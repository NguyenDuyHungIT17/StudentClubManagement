using StudentClub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.Chat
{
    /// <summary>
    /// Response DTO cho tin nhắn
    /// </summary>
    public class ChatMessageResponseDto
    {
        /// <summary>
        /// ID tin nhắn
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// ID người gửi
        /// </summary>
        public int SenderId { get; set; }

        /// <summary>
        /// Tên người gửi
        /// </summary>
        public string? SenderName { get; set; }

        /// <summary>
        /// Loại tin nhắn
        /// </summary>
        public ChatMessageType MessageType { get; set; }

        /// <summary>
        /// Trạng thái tin nhắn
        /// </summary>
        public ChatMessageStatus Status { get; set; }

        /// <summary>
        /// Nội dung tin nhắn
        /// </summary>
        public string Content { get; set; } = null!;

        /// <summary>
        /// ID CLB (nếu là tin nhắn nhóm)
        /// </summary>
        public int? ClubId { get; set; }

        /// <summary>
        /// ID người nhận (nếu là tin nhắn riêng)
        /// </summary>
        public int? RecipientId { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Đánh dấu tin nhắn bị xóa
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
