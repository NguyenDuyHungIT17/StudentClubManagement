using Microsoft.IdentityModel.Tokens;
using StudentClub.Application.DTOs.DtoRealTime;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices.IRealtimeService;
using StudentClub.Application.Realtime; // Namespace chứa WebSocketConnection
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

            // 1. Lấy Token
            var token = context.Request.Query["access_token"].ToString();
            token = token.Replace("Bearer ", "").Replace("bearer ", "").Trim();

            if (string.IsNullOrEmpty(token)) { context.Response.StatusCode = 401; return; }

            // 2. Validate và lấy thông tin (Int)
            var userContext = ResolveUserContext(context, token);
            if (userContext == null)
            {
                Console.WriteLine("❌ Auth Failed: Token không hợp lệ hoặc không parse được ID.");
                context.Response.StatusCode = 401;
                return;
            }

            // 3. Kết nối
            using var socket = await context.WebSockets.AcceptWebSocketAsync();
            var connectionId = Guid.NewGuid();

            // Resolve Service
            var manager = context.RequestServices.GetRequiredService<IRealtimeConnectionManager>();
            var chatService = context.RequestServices.GetRequiredService<IRealtimeService>();

            var conn = new WebSocketConnection
            {
                ConnectionId = connectionId,
                Socket = socket,
                UserId = userContext.UserId, // Int
                ClubId = userContext.ClubId,
                Role = userContext.Role
            };

            manager.Add(conn);

            Console.WriteLine($"✅ User {conn.UserId} (Role: {conn.Role}) connected via WS.");

            // 4. Vòng lặp nhận tin nhắn
            var buffer = new byte[1024 * 4];
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    using (var ms = new MemoryStream())
                    {
                        WebSocketReceiveResult result;
                        do
                        {
                            result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            ms.Write(buffer, 0, result.Count);
                        } while (!result.EndOfMessage);

                        if (result.MessageType == WebSocketMessageType.Close) break;

                        ms.Seek(0, SeekOrigin.Begin);
                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            var json = await reader.ReadToEndAsync();
                            try
                            {
                                var command = JsonSerializer.Deserialize<ChatCommand>(json, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });

                                if (command != null)
                                {
                                    await chatService.HandleAsync(command, userContext);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"⚠️ JSON Error: {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Socket Error: {ex.Message}");
            }
            finally
            {
                manager.Remove(connectionId);
                if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
            }
        });
    }

    private static RealtimeUserContext? ResolveUserContext(HttpContext context, string token)
    {
        try
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var validationParams = context.RequestServices.GetRequiredService<TokenValidationParameters>();

            // Validate Token
            var principal = handler.ValidateToken(token, validationParams, out _);

            // Tìm Claim UserId (Claim mặc định của .NET thường là NameIdentifier, hoặc 'sub')
            var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? principal.FindFirst("sub")?.Value;

            var role = principal.FindFirst(ClaimTypes.Role)?.Value;
            var clubIdStr = principal.FindFirst("clubId")?.Value;

            if (userIdStr == null || role == null)
            {
                Console.WriteLine("🔴 Token thiếu UserId hoặc Role.");
                return null;
            }

            // CHUẨN HÓA: Parse sang INT
            if (!int.TryParse(userIdStr, out int userId))
            {
                Console.WriteLine($"🔴 Lỗi: UserId trong token là '{userIdStr}' không phải số nguyên (int).");
                return null;
            }

            // Xử lý ClubId (Giả sử vẫn là Guid, nếu ClubId là int thì sửa nốt dòng này)
            int? clubId = null;
            if (int.TryParse(clubIdStr, out var cid)) clubId = cid;

            return new RealtimeUserContext
            {
                UserId = userId, // Int
                Role = role,
                ClubId = clubId
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Validate Token Exception: {ex.Message}");
            return null;
        }
    }
}