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
    public class InterviewRepository : IInterviewRepository
    {
        private readonly StudentClubDbContext _context;
        public InterviewRepository(StudentClubDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Interview interview)
        {
            await _context.Interviews.AddAsync(interview);
        }

        public async Task<Interview?> GetByIdAsync(int id)
        {
            return await _context.Interviews.FindAsync(id);
        }

        public async Task<List<Interview>> GetByClubIdAsync(int clubId)
        {
            return await _context.Interviews
                .Where(i => i.ClubId == clubId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Interview>> GetAllAsync()
        {
            return await _context.Interviews
               .AsNoTracking()
               .ToListAsync();
        }

        public async Task DeleteAsync(Interview interview)
        {
            _context.Interviews.Remove(interview);
            await Task.CompletedTask;
        }

        public async Task<Interview?> GetByClubIdAndEmail(int clubId, string email)
        {
            var interview = await _context.Interviews.Where(u => u.ClubId == clubId && u.ApplicantEmail == email).FirstOrDefaultAsync();
            return interview;
        }
    }

}
