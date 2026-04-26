using StudentClub.Application.Interfaces;
using StudentClub.Application.Realtime;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace StudentClub.Infrastructure.Realtime
{
    /// <summary>
    /// Manages WebSocket connections for real-time chat
    /// </summary>
    public class WebSocketConnectionManager : IRealtimeConnectionManager
    {
        private readonly ConcurrentDictionary<Guid, WebSocketConnection> _connections = new();

        /// <summary>
        /// Add a new connection
        /// </summary>
        public void Add(WebSocketConnection connection)
        {
            // Disconnect old connection of same user if exists
            var oldConnection = _connections.Values.FirstOrDefault(c => c.UserId == connection.UserId);
            if (oldConnection != null)
            {
                Remove(oldConnection.ConnectionId);
            }

            _connections[connection.ConnectionId] = connection;
            Console.WriteLine($"Added connection: {connection.ConnectionId} - User {connection.UserId}");
        }

        /// <summary>
        /// Remove a connection
        /// </summary>
        public void Remove(Guid connectionId)
        {
            if (_connections.TryRemove(connectionId, out var connection))
            {
                Console.WriteLine($"Removed connection: {connectionId} - User {connection.UserId}");
            }
        }

        /// <summary>
        /// Get connection by user ID
        /// </summary>
        public WebSocketConnection? GetByUser(int userId)
        {
            return _connections.Values.FirstOrDefault(c =>
                c.UserId == userId && c.IsConnected);
        }

        /// <summary>
        /// Get all connections in a club
        /// </summary>
        public List<WebSocketConnection> GetByClub(int clubId)
        {
            return _connections.Values
                .Where(c => c.ClubId == clubId && c.IsConnected)
                .ToList();
        }

        /// <summary>
        /// Get all leader/admin connections in a club
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
        /// Send message to a single connection
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
        /// Broadcast message to multiple connections
        /// </summary>
        public async Task BroadcastAsync(object payload, List<WebSocketConnection> connections)
        {
            if (connections.Count == 0)
                return;

            var tasks = connections.Select(c => SendAsync(c, payload)).ToList();
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Get all active connections
        /// </summary>
        public List<WebSocketConnection> GetAll()
        {
            return _connections.Values.Where(c => c.IsConnected).ToList();
        }

        /// <summary>
        /// Get count of active connections
        /// </summary>
        public int Count()
        {
            return _connections.Count(c => c.Value.IsConnected);
        }
    }
}