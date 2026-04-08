using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.Chat
{
    /// <summary>
    /// Request DTO để lấy tin nhắn nhóm CLB
    /// </summary>
    public class GetGroupMessagesRequestDto
    {
        /// <summary>
        /// ID CLB
        /// </summary>
        public int ClubId { get; set; }

        /// <summary>
        /// Số trang
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Số item mỗi trang
        /// </summary>
        public int PageSize { get; set; } = 50;
    }
}
