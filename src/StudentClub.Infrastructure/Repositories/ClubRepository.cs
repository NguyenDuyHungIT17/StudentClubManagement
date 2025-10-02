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
    public class ClubRepository : IClubRepository
    {
        private readonly StudentClubDbContext _context;
        public ClubRepository(StudentClubDbContext context)
        {
            _context = context;
        }

        public async Task AddClubAsync(Club club)
        {
            await _context.Clubs.AddAsync(club);
        }

        public async Task<Club> GetClubByClubIdAsync(int id)
        {
            return await _context.Clubs.FirstOrDefaultAsync(c => c.ClubId == id);
        }

        public async Task<Club> GetClubByClubNameAsync(string clubName)
        {
            return await _context.Clubs.FirstOrDefaultAsync(c => c.ClubName == clubName);
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClubAsync(Club club)
        {
             _context.Clubs.Update(club);
        }

        public async Task UpdateLeaderIdAsync(int clubId, int leaderId)
        {
            var club = await _context.Clubs.FirstOrDefaultAsync(c => c.ClubId == clubId);
            
            club.LeaderId = leaderId;
            club.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteEventRegistrationsByClubIdAsync(int clubId)
        {
            var registrations = _context.EventRegistrations
                .Where(r => r.Event.ClubId == clubId);
            _context.EventRegistrations.RemoveRange(registrations);
        }

        public async Task DeleteFeedbacksByClubIdAsync(int clubId)
        {
            var feedbacks = _context.Feedbacks
                .Where(f => f.Event.ClubId == clubId);
            _context.Feedbacks.RemoveRange(feedbacks);
        }

        public async Task DeleteEventsByClubIdAsync(int clubId)
        {
            var events = _context.Events.Where(e => e.ClubId == clubId);
            _context.Events.RemoveRange(events);
        }

        public async Task DeleteMembersByClubIdAsync(int clubId)
        {
            var members = _context.ClubMembers.Where(m => m.ClubId == clubId);
            _context.ClubMembers.RemoveRange(members);
        }

        public async Task DeleteInterviewsByClubIdAsync(int clubId)
        {
            var interviews = _context.Interviews.Where(i => i.ClubId == clubId);
            _context.Interviews.RemoveRange(interviews);
        }

        public async Task DeleteClubAsync(Club club)
        {
            _context.Clubs.Remove(club);
        }

        public async Task<List<Club>> GetClubsAsync()
        {
            return await _context.Clubs.ToListAsync();
        }

       

        public async Task<Club> GetClubAsync(int id)
        {
            var club = await _context.Clubs.FirstOrDefaultAsync(x => x.ClubId == id);
            return club;
        }

        public async Task<string?> GetCLubNameByClubIdAsync(int clubId)
        {
            var clubName = await _context.Clubs.Where(c => c.ClubId == clubId).Select( c => c.ClubName).FirstOrDefaultAsync();
            return clubName;
        }
    }
}
