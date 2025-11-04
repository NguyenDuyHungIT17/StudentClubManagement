using Microsoft.EntityFrameworkCore;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using StudentClub.Infrastructure.Persistence;

namespace StudentClub.Infrastructure.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly StudentClubDbContext _context;

        public FeedbackRepository(StudentClubDbContext context)
        {
            _context = context;
        }

        public async Task CreateFeedbackAsync(Feedback feedback)
        {
            await _context.AddRangeAsync(feedback);
        }

        public async Task DeleteFeedbackAsync(int feedbackId)
        {
            var feedback = _context.Feedbacks.Where(f => f.FeedbackId == feedbackId);
            _context.Feedbacks.RemoveRange(feedback);
        }

        public async Task<List<Feedback>> GetAllFeedbacksAsync()
        {
            return await _context.Feedbacks.ToListAsync();
        }

        public Task<Feedback> GetFeedbackByIdAsync(int feedbackId)
        {
            return _context.Feedbacks.FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);
        }

        public Task<List<Feedback>> GetFeedbacksByEventIdAsync(int eventId)
        {
            return _context.Feedbacks.Where(f => f.EventId == eventId).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFeedbackAsync(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
        }
    }
}
