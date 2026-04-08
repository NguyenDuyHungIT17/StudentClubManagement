using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Domain.Entities.Realtime
{
    [Table("ChatUnreadMessages")]
    public class ChatUnreadMessage
    {
        [Key]
        public int UnreadId { get; set; }

        /// <summary>
        /// User nhận tin nhắn
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Tin nhắn chưa đọc
        /// </summary>
        [Required]
        public int MessageId { get; set; }

        /// <summary>
        /// Thời gian người này nhận tin
        /// </summary>
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Thời gian người này đọc tin
        /// </summary>
        public DateTime? ReadAt { get; set; }

        /// <summary>
        /// Đánh dấu tin đã đọc hay chưa
        /// </summary>
        public bool IsRead { get; set; } = false;

    }
}
