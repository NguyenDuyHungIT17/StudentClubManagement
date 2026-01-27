using StudentClub.Application.DTOs.DtoRealTime;
using System.Net.WebSockets;

namespace StudentClub.Application.Realtime
{
    public class WebSocketConnection : RealtimeConnection
    {
        public WebSocket Socket { get; set; } = default!;
    }
}
