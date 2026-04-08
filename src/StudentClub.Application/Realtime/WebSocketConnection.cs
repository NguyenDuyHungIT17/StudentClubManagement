using System.Net.WebSockets;

namespace StudentClub.Application.Realtime
{
    /// <summary>
    /// Model đại diện cho 1 kết nối WebSocket
    /// </summary>
    public class WebSocketConnection
    {
        /// <summary>
        /// ID unique cho mỗi connection
        /// </summary>
        public Guid ConnectionId { get; set; }

        /// <summary>
        /// WebSocket object
        /// </summary>
        public WebSocket Socket { get; set; } = null!;

        /// <summary>
        /// ID user kết nối
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// ID CLB (nếu có)
        /// </summary>
        public int? ClubId { get; set; }

        /// <summary>
        /// Role của user (admin, leader, member, guest)
        /// </summary>
        public string Role { get; set; } = null!;

        /// <summary>
        /// Thời gian kết nối
        /// </summary>
        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Full name của user (fetch từ DB)
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Trạng thái kết nối
        /// </summary>
        public bool IsConnected => Socket?.State == WebSocketState.Open;
    }
}