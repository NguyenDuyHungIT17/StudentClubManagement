using StudentClub.Application.Realtime;

namespace StudentClub.Application.Interfaces
{
    /// <summary>
    /// Interface quản lý WebSocket connections
    /// </summary>
    public interface IRealtimeConnectionManager
    {
        /// <summary>
        /// Thêm connection mới
        /// </summary>
        void Add(WebSocketConnection connection);

        /// <summary>
        /// Xóa connection
        /// </summary>
        void Remove(Guid connectionId);

        /// <summary>
        /// Lấy connection của user
        /// </summary>
        WebSocketConnection? GetByUser(int userId);

        /// <summary>
        /// Lấy tất cả connection của 1 CLB
        /// </summary>
        List<WebSocketConnection> GetByClub(int clubId);

        /// <summary>
        /// Lấy tất cả leader/admin của 1 CLB
        /// </summary>
        List<WebSocketConnection> GetLeaders(int clubId);

        /// <summary>
        /// Gửi message đến 1 connection
        /// </summary>
        Task SendAsync(WebSocketConnection connection, object payload);

        /// <summary>
        /// Lấy tất cả connections đang active
        /// </summary>
        List<WebSocketConnection> GetAll();

        /// <summary>
        /// Số lượng connections hiện tại
        /// </summary>
        int Count();
    }
}