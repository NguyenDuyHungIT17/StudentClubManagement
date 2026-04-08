using Microsoft.IdentityModel.Tokens;
using StudentClub.Application.DTOs.DtoRealTime;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices.IRealtimeService;
using StudentClub.Application.Realtime;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace StudentClub.API.WebSockets
{
    /// <summary>
    /// WebSocket endpoint cho real-time chat
    /// </summary>
    public static class ChatWebSocketEndpoint
    {
        /// <summary>
        /// Map WebSocket endpoint /ws/chat
        /// </summary>
        public static void MapChat(WebApplication app)
        {
            app.Map("/ws/chat", async (HttpContext context) =>
            {
                // ============================================
                // 1. Validate WebSocket request
                // ============================================
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("WebSocket request expected");
                    return;
                }

                // ============================================
                // 2. Extract & validate token
                // ============================================
                var token = context.Request.Query["access_token"].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    token = ExtractTokenFromAuthHeader(context);
                }

                if (string.IsNullOrEmpty(token))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Authorization token required");
                    return;
                }

                // ============================================
                // 3. Resolve user context from token
                // ============================================
                var userContext = ResolveUserContext(context, token);
                if (userContext == null)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid token");
                    return;
                }

                // ============================================
                // 4. Accept WebSocket connection
                // ============================================
                using var socket = await context.WebSockets.AcceptWebSocketAsync();
                var connectionId = Guid.NewGuid();

                // ============================================
                // 5. Resolve services
                // ============================================
                var connectionManager = context.RequestServices.GetRequiredService<IRealtimeConnectionManager>();
                var realtimeService = context.RequestServices.GetRequiredService<IRealtimeService>();

                // ============================================
                // 6. Create & register connection
                // ============================================
                var connection = new WebSocketConnection
                {
                    ConnectionId = connectionId,
                    Socket = socket,
                    UserId = userContext.UserId,
                    ClubId = userContext.ClubId,
                    Role = userContext.Role,
                    ConnectedAt = DateTime.UtcNow
                };

                connectionManager.Add(connection);
                Console.WriteLine($"✅ WebSocket connected: User {connection.UserId} ({connection.Role})");

                // ============================================
                // 7. Message receive loop
                // ============================================
                var buffer = new byte[1024 * 4];
                try
                {
                    while (socket.State == WebSocketState.Open)
                    {
                        using (var ms = new MemoryStream())
                        {
                            WebSocketReceiveResult result;

                            // Receive message
                            do
                            {
                                result = await socket.ReceiveAsync(
                                    new ArraySegment<byte>(buffer),
                                    CancellationToken.None);
                                ms.Write(buffer, 0, result.Count);
                            } while (!result.EndOfMessage);

                            // Handle close frame
                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                break;
                            }

                            // Parse message
                            ms.Seek(0, SeekOrigin.Begin);
                            using (var reader = new StreamReader(ms, Encoding.UTF8))
                            {
                                var json = await reader.ReadToEndAsync();
                                try
                                {
                                    var command = JsonSerializer.Deserialize<ChatCommand>(json,
                                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                                    if (command != null && !string.IsNullOrWhiteSpace(command.Content))
                                    {
                                        // Handle the message
                                        await realtimeService.HandleAsync(command, userContext);
                                    }
                                }
                                catch (JsonException ex)
                                {
                                    Console.WriteLine($"⚠️ JSON parsing error: {ex.Message}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ WebSocket error: {ex.Message}");
                }
                finally
                {
                    // ============================================
                    // 8. Cleanup on disconnect
                    // ============================================
                    connectionManager.Remove(connectionId);
                    Console.WriteLine($"❌ WebSocket disconnected: User {userContext.UserId}");

                    if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
                    {
                        await socket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Closed",
                            CancellationToken.None);
                    }

                    socket.Dispose();
                }
            });
        }

        /// <summary>
        /// Resolve user context from JWT token
        /// </summary>
        private static RealtimeUserContext? ResolveUserContext(HttpContext context, string token)
        {
            try
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var validationParams = context.RequestServices.GetRequiredService<TokenValidationParameters>();

                // Validate token
                var principal = handler.ValidateToken(token, validationParams, out _);

                // Extract claims
                var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                ?? principal.FindFirst("sub")?.Value;
                var role = principal.FindFirst(ClaimTypes.Role)?.Value;
                var clubIdStr = principal.FindFirst("clubId")?.Value;

                // Validate required claims
                if (string.IsNullOrEmpty(userIdStr) || string.IsNullOrEmpty(role))
                {
                    Console.WriteLine("❌ Token missing required claims");
                    return null;
                }

                // Parse UserId
                if (!int.TryParse(userIdStr, out int userId))
                {
                    Console.WriteLine($"❌ Invalid UserId format: {userIdStr}");
                    return null;
                }

                // Parse ClubId
                int? clubId = null;
                if (!string.IsNullOrEmpty(clubIdStr) && int.TryParse(clubIdStr, out var cid))
                {
                    clubId = cid;
                }

                return new RealtimeUserContext
                {
                    UserId = userId,
                    Role = role,
                    ClubId = clubId
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Token validation error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Extract token from Authorization header
        /// </summary>
        private static string? ExtractTokenFromAuthHeader(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader))
                return null;

            const string bearerScheme = "Bearer ";
            if (authHeader.StartsWith(bearerScheme, StringComparison.OrdinalIgnoreCase))
            {
                return authHeader.Substring(bearerScheme.Length).Trim();
            }

            return null;
        }
    }
}