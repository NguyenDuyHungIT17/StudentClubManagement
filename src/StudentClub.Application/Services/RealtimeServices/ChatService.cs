using StudentClub.Application.DTOs.DtoRealTime;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices.IRealtimeService;

namespace StudentClub.Application.Services.RealtimeServices
{
    public class ChatService : IRealtimeService
    {
        private readonly IRealtimeConnectionManager _connections;

        public ChatService(IRealtimeConnectionManager connections)
        {
            _connections = connections;
        }

        public async Task HandleAsync(ChatCommand cmd, RealtimeUserContext user)
        {
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
            }
        }

        private async Task HandleGroup(ChatCommand cmd, RealtimeUserContext user)
        {
            var conns = _connections.GetByClub(cmd.ClubId);

            foreach (var c in conns)
            {
                await _connections.SendAsync(c, new
                {
                    type = "GROUP_MESSAGE",
                    fromUserId = user.UserId,
                    content = cmd.Content
                });
            }
        }

        private async Task HandlePrivate(ChatCommand cmd, RealtimeUserContext user)
        {
            var target = _connections.GetByUser(cmd.ToUserId!.Value);
            if (target == null) return;

            await _connections.SendAsync(target, new
            {
                type = "PRIVATE_MESSAGE",
                fromUserId = user.UserId,
                content = cmd.Content
            });
        }

        private async Task HandleGuest(ChatCommand cmd)
        {
            var leaders = _connections.GetLeaders(cmd.ClubId);

            foreach (var leader in leaders)
            {
                await _connections.SendAsync(leader, new
                {
                    type = "GUEST_MESSAGE",
                    content = cmd.Content
                });
            }
        }
    }
}
