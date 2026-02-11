using StudentClub.Application.DTOs.DtoRealTime;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices.IRealtimeService;

namespace StudentClub.Application.Services.RealtimeServices
{
    public class ChatService : IRealtimeService
    {
        private readonly IRealtimeConnectionManager _connections;

        // Nếu bạn muốn lưu tin nhắn vào DB, hãy Inject thêm Repository vào đây
        // private readonly IChatRepository _chatRepo; 

        public ChatService(IRealtimeConnectionManager connections)
        {
            _connections = connections;
        }

        public async Task HandleAsync(ChatCommand cmd, RealtimeUserContext user)
        {
            // TODO: Tại đây bạn nên gọi Repository để lưu tin nhắn vào Database trước khi gửi đi
            // await _chatRepo.AddAsync(new ChatMessage { ... });

            switch (cmd.Type)
            {
                case "CLUB_GROUP":
                    await HandleGroup(cmd, user);
                    break;

                case "PRIVATE":
                    await HandlePrivate(cmd, user);
                    break;

                case "GUEST_TO_LEADER":
                    await HandleGuest(cmd);
                    break;

                default:
                    // Log cảnh báo nếu nhận type lạ
                    Console.WriteLine($"Unknown message type: {cmd.Type}");
                    break;
            }
        }

        private async Task HandleGroup(ChatCommand cmd, RealtimeUserContext user)
        {
            var conns = _connections.GetByClub(cmd.ClubId);

            // 1. Tạo gói tin 1 lần duy nhất để tiết kiệm bộ nhớ
            var payload = new
            {
                type = "GROUP_MESSAGE",
                fromUserId = user.UserId,
                role = user.Role, // Thêm role để FE biết là Admin hay Member chat
                content = cmd.Content,
                timestamp = DateTime.UtcNow
            };

            // 2. Gửi song song (Parallel) thay vì tuần tự
            // Giúp không bị tắc nghẽn nếu một client mạng yếu
            var tasks = conns.Select(c => _connections.SendAsync(c, payload));

            await Task.WhenAll(tasks);
        }

        private async Task HandlePrivate(ChatCommand cmd, RealtimeUserContext user)
        {
            if (cmd.ToUserId == null) return;

            var target = _connections.GetByUser(cmd.ToUserId.Value);

            // Nếu người nhận đang online
            if (target != null)
            {
                var payload = new
                {
                    type = "PRIVATE_MESSAGE",
                    fromUserId = user.UserId,
                    content = cmd.Content,
                    timestamp = DateTime.UtcNow
                };

                await _connections.SendAsync(target, payload);
            }

            // Note: Thông thường chat riêng thì Frontend sẽ tự hiển thị tin nhắn của chính mình
            // nên không cần gửi ngược lại cho Sender.
        }

        private async Task HandleGuest(ChatCommand cmd)
        {
            var leaders = _connections.GetLeaders(cmd.ClubId);

            var payload = new
            {
                type = "GUEST_MESSAGE",
                content = cmd.Content,
                timestamp = DateTime.UtcNow
            };

            // Gửi song song cho tất cả Leader/Admin của CLB đó
            var tasks = leaders.Select(leader => _connections.SendAsync(leader, payload));

            await Task.WhenAll(tasks);
        }
    }
}