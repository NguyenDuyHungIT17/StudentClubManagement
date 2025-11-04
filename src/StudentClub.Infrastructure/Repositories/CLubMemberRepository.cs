using Microsoft.EntityFrameworkCore;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using StudentClub.Infrastructure.Persistence;

namespace StudentClub.Infrastructure.Repositories
{
    public class CLubMemberRepository : IClubMemberRepository
    {
        private readonly StudentClubDbContext _context;
        public CLubMemberRepository(StudentClubDbContext context)
        {
            _context = context;
        }

        public async Task AddClubMemberAsync(ClubMember clubMember)
        {
            await _context.AddAsync(clubMember);
        }

        public async Task<int> GetClubIdByUserId(int userId)
        {
            var clubMember = await _context.ClubMembers.Where(u => u.UserId == userId).FirstOrDefaultAsync();
            return clubMember.ClubId;
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
