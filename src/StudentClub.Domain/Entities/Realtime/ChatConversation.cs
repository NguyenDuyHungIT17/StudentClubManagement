using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Domain.Entities.Realtime
{
    [Table("ChatConversations")]
    public class ChatConversation
    {
        [Key]
        public int ConversationId { get; set; }

        /// <summary>
        /// User 1 (ID nhỏ hơn - để tránh duplicate conversation)
        /// </summary>
        [Required]
        public int User1Id { get; set; }

        /// <summary>
        /// User 2 (ID lớn hơn)
        /// </summary>
        [Required]
        public int User2Id { get; set; }

        /// <summary>
        /// Tin nhắn cuối cùng trong cuộc trò chuyện
        /// </summary>
        public int? LastMessageId { get; set; }

        /// <summary>
        /// Thời gian tin nhắn cuối cùng
        /// </summary>
        public DateTime? LastMessageTime { get; set; }

        /// <summary>
        /// Cuộc trò chuyện còn active không
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Thời gian tạo cuộc trò chuyện
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Thời gian cập nhật cuộc trò chuyện
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
