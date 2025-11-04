using Microsoft.EntityFrameworkCore;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using StudentClub.Infrastructure.Persistence;

namespace StudentClub.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly StudentClubDbContext _context;

        public EventRepository(StudentClubDbContext context)
        {
            _context = context;
        }
        public async Task AddEventAsync(Event e)
        {
            await _context.Events.AddAsync(e);
        }

        public async Task DeleteEvent(int id)
        {
            var feedbacks = _context.Feedbacks
                .Where(f => f.Event.EventId == id);
            _context.Feedbacks.RemoveRange(feedbacks);

            var regisrtation = _context.EventRegistrations
                    .Where(f => f.Event.EventId == id);
            _context.EventRegistrations.RemoveRange(regisrtation);

            var ev = _context.Events.Where(e => e.EventId == id);
            _context.Events.RemoveRange(ev);
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event?> GetByEventIdAsync(int eventId)
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            return ev;
        }

        public async Task<Event?> GetEventByIdAsync(int eventId)
        {
           return await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
        }

        public async Task<string> GetEventNameByIdAsync(int eventId)
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            if (ev == null)
            {
                throw new Exception("Event not found");
            }
            return ev.Title;
        }

        public async Task<List<Event>> GetEventsByCLubIdAsync(int clubId)
        {
            return await _context.Events.Where(e => e.ClubId == clubId).ToListAsync();
        }

        public async Task<List<Event>> GetPublicEventsAsync(bool check)
        {
            return await _context.Events.Where(e => e.IsPrivate == check).ToListAsync();
        }

        public Task<List<Event>> GetPublicEventsByCLubIdAsync(int clubId, bool check)
        {
            return _context.Events.Where(e => e.ClubId == clubId && e.IsPrivate == check).ToListAsync();
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
