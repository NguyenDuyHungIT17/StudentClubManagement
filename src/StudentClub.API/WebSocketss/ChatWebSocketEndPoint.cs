using Microsoft.IdentityModel.Tokens;
using StudentClub.Application.DTOs.DtoRealTime;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices.IRealtimeService;
using StudentClub.Application.Realtime;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace StudentClub.API.WebSockets;

public static class ChatWebSocketEndpoint
{
    public static void MapChat(WebApplication app)
    {
        app.Map("/ws/chat", async (HttpContext context) =>
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = 400;
                return;
            }

            var token = context.Request.Query["access_token"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 401;
                return;
            }

            var userContext = ResolveUserContext(context, token);
            if (userContext == null)
            {
                context.Response.StatusCode = 401;
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            var connectionId = Guid.NewGuid();

            var manager = context.RequestServices
                .GetRequiredService<IRealtimeConnectionManager>();

            var chatService = context.RequestServices
                .GetRequiredService<IRealtimeService>();

            var conn = new WebSocketConnection
            {
                ConnectionId = connectionId,
                Socket = socket,
                UserId = userContext.UserId,
                ClubId = userContext.ClubId,
                Role = userContext.Role
            };

            manager.Add(conn);

            var buffer = new byte[4096];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var command = JsonSerializer.Deserialize<ChatCommand>(json);

                    if (command == null) continue;

                    await chatService.HandleAsync(command, userContext);
                }
            }
            finally
            {
                manager.Remove(connectionId);
                await socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Closed",
                    CancellationToken.None);
            }
        });
    }

    private static RealtimeUserContext? ResolveUserContext(
        HttpContext context,
        string token)
    {
        try
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

            var validationParams = context.RequestServices
                .GetRequiredService<TokenValidationParameters>();

            var principal = handler.ValidateToken(token, validationParams, out _);

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value;
            var clubId = principal.FindFirst("clubId")?.Value;

            if (userId == null || role == null)
                return null;

            return new RealtimeUserContext
            {
                UserId = Guid.Parse(userId),
                Role = role,
                ClubId = clubId != null ? Guid.Parse(clubId) : null
            };
        }
        catch
        {
            return null;
        }
    }
}
