using StudentClub.Application.DTOs.DtoRealTime;
using StudentClub.Application.Interfaces;
using StudentClub.Application.Realtime;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace StudentClub.Infrastructure.Realtime
{
    public class WebSocketConnectionManager : IRealtimeConnectionManager
    {
        private readonly ConcurrentDictionary<Guid, WebSocketConnection> _connections = new();

        public void Add(WebSocketConnection conn)
            => _connections[conn.ConnectionId] = conn;

        public void Remove(Guid id)
            => _connections.TryRemove(id, out _);

        public IEnumerable<RealtimeConnection> GetByClub(Guid clubId)
            => _connections.Values.Where(c => c.ClubId == clubId);

        public RealtimeConnection? GetByUser(Guid userId)
            => _connections.Values.FirstOrDefault(c => c.UserId == userId);

        public IEnumerable<RealtimeConnection> GetLeaders(Guid clubId)
            => _connections.Values.Where(c => c.ClubId == clubId && c.Role == "Leader");

        public async Task SendAsync(RealtimeConnection conn, object payload)
        {
            var wsConn = (WebSocketConnection)conn;

            if (wsConn.Socket.State != WebSocketState.Open)
                return;

            var json = JsonSerializer.Serialize(payload);
            var bytes = Encoding.UTF8.GetBytes(json);

            await wsConn.Socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}

