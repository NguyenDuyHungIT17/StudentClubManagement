using StudentClub.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.IServices
{
    public interface IEventService
    {
        Task<CreateEventResponseDto> CreateEventAsync(CreateEventRequestDto request, int userId, string role);
        Task<CreateEventResponseDto> UpdateEventAsync(UpdateEventRequestDto requestDto, int eventId, int userId, string role);
    }
}
