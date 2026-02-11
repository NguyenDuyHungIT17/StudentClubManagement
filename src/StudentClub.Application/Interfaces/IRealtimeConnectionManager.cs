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
        IEnumerable<RealtimeConnection> GetByClub(int clubId);
        RealtimeConnection? GetByUser(int userId);
        IEnumerable<RealtimeConnection> GetLeaders(int clubId);
        Task SendAsync(RealtimeConnection connection, object payload);
        void Add(WebSocketConnection conn);
        void Remove(Guid id);

    }
}
