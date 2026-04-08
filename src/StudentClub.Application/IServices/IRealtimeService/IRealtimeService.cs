using StudentClub.Application.DTOs.DtoRealTime;
using StudentClub.Application.Realtime;

namespace StudentClub.Application.IServices.IRealtimeService
{
    /// <summary>
    /// Interface xử lý real-time chat
    /// </summary>
    public interface IRealtimeService
    {
        /// <summary>
        /// Xử lý tin nhắn từ WebSocket
        /// </summary>
        Task HandleAsync(ChatCommand cmd, RealtimeUserContext user);

        /// <summary>
        /// Gửi broadcast tin nhắn (helper method)
        /// </summary>
        Task BroadcastAsync(object payload, List<WebSocketConnection> connections);
    }
}