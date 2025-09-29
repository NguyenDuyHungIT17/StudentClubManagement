using Microsoft.EntityFrameworkCore;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using StudentClub.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Event?> GetByEventIdAsync(int eventId)
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            return ev;
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
