using StudentClub.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.IServices
{
    public interface IEventRegistrationService
    {
        Task<CreateEventRegistrationResponseDto> CreateEventRegistrationAsync(CreateEventRegistrationRequestDto request);
        Task DeleteEventRegistration(int eventRegistrationId, string role, int userId);
        Task<List<CreateEventRegistrationResponseDto>> GetAllEventRegistrationsByEventId(int eventId);
    }
}
