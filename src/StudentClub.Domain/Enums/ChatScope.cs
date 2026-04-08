using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Domain.Enums
{
    /// <summary>
    /// Enum định nghĩa các loại tin nhắn trong hệ thống chat real-time
    /// </summary>
    public enum ChatMessageType
    {
        /// <summary>
        /// Tin nhắn nhóm CLB (gửi cho tất cả thành viên)
        /// </summary>
        GroupMessage = 1,

        /// <summary>
        /// Tin nhắn riêng tư giữa 2 user (1-1 private chat)
        /// </summary>
        PrivateMessage = 2,

        /// <summary>
        /// Tin nhắn từ khách (guest) tới leader/admin CLB
        /// </summary>
        GuestMessage = 3
    }

    /// <summary>
    /// Enum định nghĩa trạng thái tin nhắn trong chat
    /// </summary>
    public enum ChatMessageStatus
    {
        /// <summary>
        /// Tin nhắn đã gửi
        /// </summary>
        Sent = 1,

        /// <summary>
        /// Tin nhắn đã giao (server nhận)
        /// </summary>
        Delivered = 2,

        /// <summary>
        /// Tin nhắn đã đọc
        /// </summary>
        Read = 3,

        /// <summary>
        /// Tin nhắn bị xóa
        /// </summary>
        Deleted = 4
    }


    /// <summary>
    /// Enum định nghĩa các vai trò người dùng (dùng cho phân quyền trong chat)
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Admin quản trị hệ thống
        /// </summary>
        Admin = 1,

        /// <summary>
        /// Leader quản lý CLB
        /// </summary>
        Leader = 2,

        /// <summary>
        /// Thành viên CLB
        /// </summary>
        Member = 3,

        /// <summary>
        /// Khách (chưa gia nhập)
        /// </summary>
        Guest = 4
    }


}
