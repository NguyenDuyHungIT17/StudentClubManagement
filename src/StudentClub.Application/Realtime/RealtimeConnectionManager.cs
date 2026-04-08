using StudentClub.Application.Interfaces;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace StudentClub.Application.Realtime
{
    /// <summary>
    /// Quản lý WebSocket connections
    /// </summary>
    public class RealtimeConnectionManager : IRealtimeConnectionManager
    {
        private readonly ConcurrentDictionary<Guid, WebSocketConnection> _connections =
            new ConcurrentDictionary<Guid, WebSocketConnection>();

        /// <summary>
        /// Thêm connection mới
        /// </summary>
        public void Add(WebSocketConnection connection)
        {
            // Disconnect connection cũ của user nếu có
            var oldConnection = _connections.Values.FirstOrDefault(c => c.UserId == connection.UserId);
            if (oldConnection != null)
            {
                Remove(oldConnection.ConnectionId);
            }

            _connections.TryAdd(connection.ConnectionId, connection);
            Console.WriteLine($"Added connection: {connection.ConnectionId} - User {connection.UserId}");
        }

        /// <summary>
        /// Xóa connection
        /// </summary>
        public void Remove(Guid connectionId)
        {
            if (_connections.TryRemove(connectionId, out var connection))
            {
                Console.WriteLine($"❌ Removed connection: {connectionId} - User {connection.UserId}");
            }
        }

        /// <summary>
        /// Lấy connection của user
        /// </summary>
        public WebSocketConnection? GetByUser(int userId)
        {
            return _connections.Values.FirstOrDefault(c => c.UserId == userId && c.IsConnected);
        }

        /// <summary>
        /// Lấy tất cả connection của 1 CLB
        /// </summary>
        public List<WebSocketConnection> GetByClub(int clubId)
        {
            return _connections.Values
                .Where(c => c.ClubId == clubId && c.IsConnected)
                .ToList();
        }

        /// <summary>
        /// Lấy tất cả leader/admin của 1 CLB
        /// </summary>
        public List<WebSocketConnection> GetLeaders(int clubId)
        {
            return _connections.Values
                .Where(c => c.ClubId == clubId &&
                       (c.Role == "admin" || c.Role == "leader") &&
                       c.IsConnected)
                .ToList();
        }

        /// <summary>
        /// Gửi message đến 1 connection
        /// </summary>
        public async Task SendAsync(WebSocketConnection connection, object payload)
        {
            try
            {
                if (connection?.Socket?.State != WebSocketState.Open)
                {
                    Remove(connection?.ConnectionId ?? Guid.Empty);
                    return;
                }

                var json = JsonSerializer.Serialize(payload);
                var bytes = Encoding.UTF8.GetBytes(json);

                await connection.Socket.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Send error: {ex.Message}");
                Remove(connection?.ConnectionId ?? Guid.Empty);
            }
        }

        /// <summary>
        /// Lấy tất cả connections đang active
        /// </summary>
        public List<WebSocketConnection> GetAll()
        {
            return _connections.Values.Where(c => c.IsConnected).ToList();
        }

        /// <summary>
        /// Số lượng connections hiện tại
        /// </summary>
        public int Count()
        {
            return _connections.Count(c => c.Value.IsConnected);
        }
    }
}