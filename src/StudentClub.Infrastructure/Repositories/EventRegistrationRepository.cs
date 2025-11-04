using Microsoft.EntityFrameworkCore;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using StudentClub.Infrastructure.Persistence;

namespace StudentClub.Infrastructure.Repositories
{
    public class EventRegistrationRepository : IEventRegistrationRepository
    {
        private readonly StudentClubDbContext _context;

        public EventRegistrationRepository(StudentClubDbContext context)
        {
            _context = context;
        }
        public async Task AddEventRegistrationAsync(EventRegistration eventRegistration)
        {
            await _context.EventRegistrations.AddAsync(eventRegistration);
        }

        public async Task DeleteEventRegistrationAsync(int eventRegistritonId)
        {
            var eventRegistration =  _context.EventRegistrations.Where(e => e.RegistrationId == eventRegistritonId);
            _context.EventRegistrations.RemoveRange(eventRegistration);
        }

        public async Task<EventRegistration?> GetEventRegistrationByIdAsync(int eventRegistrationId)
        {
            var eventRegistration = await _context.EventRegistrations.FirstOrDefaultAsync(e => e.RegistrationId == eventRegistrationId);
            return eventRegistration;
        }

        public  Task UpdateEventRegistrationAsync(EventRegistration eventRegistration)
        {
            _context.EventRegistrations.Update(eventRegistration);
            return Task.CompletedTask;
        }

        public async Task SaveChangeAsynce()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<EventRegistration?>> GetEventRegistrationByEventIdAsync(int eventId)
        {
            var eventRegistration = await _context.EventRegistrations.Where(e => e.EventId == eventId).ToListAsync();
            return eventRegistration;
        }
    }
}
