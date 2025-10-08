using StudentClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.Interfaces
{
    public interface IEventRegistrationRepository
    {
        Task AddEventRegistrationAsync( EventRegistration eventRegistration);
        Task UpdateEventRegistrationAsync(EventRegistration eventRegistration);
        Task DeleteEventRegistrationAsync(int eventRegistritonId);
        Task<EventRegistration?> GetEventRegistrationByIdAsync(int eventRegistrationId);
        Task<List<EventRegistration?>> GetEventRegistrationByEventIdAsync(int eventId);
        Task SaveChangeAsynce();
    }
}
