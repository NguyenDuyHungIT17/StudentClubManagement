using StudentClub.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentClub.Domain.Entities.Realtime
{
    //lưu trữ tin nhắn
    [Table("ChatMessages")]
    public class ChatMessage
    {
        [Key]
        public int MessageId { get; set; }

        /// <summary>
        /// ID người gửi tin nhắn
        /// </summary>
        [Required]
        public int SenderId { get; set; }

        /// <summary>
        /// Loại tin nhắn (Group, Private, Guest)
        /// </summary>
        [Required]
        public ChatMessageType MessageType { get; set; }

        /// <summary>
        /// Trạng thái tin nhắn (Sent, Delivered, Read, Deleted)
        /// </summary>
        public ChatMessageStatus Status { get; set; } = ChatMessageStatus.Sent;

        /// <summary>
        /// Nội dung tin nhắn
        /// </summary>
        [Required]
        [MaxLength(5000)]
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
        /// Thời gian tạo tin nhắn
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Đánh dấu tin nhắn bị xóa (soft delete)
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Thời gian xóa tin nhắn
        /// </summary>
        public DateTime? DeletedAt { get; set; }

    }
}
