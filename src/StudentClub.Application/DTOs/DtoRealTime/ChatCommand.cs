using StudentClub.Domain.Enums;

namespace StudentClub.Application.DTOs.DtoRealTime
{
    /// <summary>
    /// Command gửi từ WebSocket client
    /// </summary>
    public class ChatCommand
    {
        /// <summary>
        /// Loại tin nhắn (1=Group, 2=Private, 3=Guest)
        /// </summary>
        public ChatMessageType Type { get; set; }

        /// <summary>
        /// ID CLB (khi Type = GroupMessage)
        /// </summary>
        public int? ClubId { get; set; }

        /// <summary>
        /// ID người nhận (khi Type = PrivateMessage)
        /// </summary>
        public int? ToUserId { get; set; }

        /// <summary>
        /// Nội dung tin nhắn
        /// </summary>
        public string Content { get; set; } = null!;
    }
}