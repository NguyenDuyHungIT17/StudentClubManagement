using StudentClub.Application.DTOs.DtoRealTime;
using StudentClub.Application.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.Interfaces
{
    public interface IRealtimeConnectionManager
    {
        IEnumerable<RealtimeConnection> GetByClub(Guid clubId);
        RealtimeConnection? GetByUser(Guid userId);
        IEnumerable<RealtimeConnection> GetLeaders(Guid clubId);
        Task SendAsync(RealtimeConnection connection, object payload);
        void Add(WebSocketConnection conn);
        void Remove(Guid id);

    }
}
