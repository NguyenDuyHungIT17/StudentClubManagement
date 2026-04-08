using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.Chat
{
    /// <summary>
    /// Response DTO cho số tin chưa đọc
    /// </summary>
    public class UnreadCountResponseDto
    {
        /// <summary>
        /// Tổng số tin chưa đọc
        /// </summary>
        public int UnreadCount { get; set; }
    }
}
