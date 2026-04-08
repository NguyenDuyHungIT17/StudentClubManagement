using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.Chat
{
    /// <summary>
    /// tạo tin nhắn
    /// </summary>
    public class CreateChatMessageRequestDto
    {
        /// <summary>
        /// Loại tin nhắn (1=Group, 2=Private, 3=Guest)
        /// </summary>
        public int MessageType { get; set; }

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
    }
}
