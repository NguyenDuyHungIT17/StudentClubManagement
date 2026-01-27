using StudentClub.Application.DTOs.DtoRealTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.IServices.IRealtimeService
{
    public interface IRealtimeService
    {
        Task HandleAsync(ChatCommand cmd, RealtimeUserContext user);
    }
}
