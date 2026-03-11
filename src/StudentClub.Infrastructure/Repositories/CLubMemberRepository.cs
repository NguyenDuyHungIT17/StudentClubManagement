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
            await _context.ClubMembers.AddAsync(clubMember);
        }

        public async Task Delete(int id)
        {
            var cl = _context.ClubMembers.Where(x => x.ClubMemberId == id).FirstOrDefault();
            _context.ClubMembers.RemoveRange(cl);
        }

        public async Task<List<ClubMember>> GetAllClubMemberAsync()
        {
            var clubMembers = await _context.ClubMembers
                .Include(x => x.Club)
                .Include(x => x.User).ToListAsync();
            return clubMembers;
        }

        public async Task<int> GetClubIdByUserId(int userId)
        {
            var clubMember = await _context.ClubMembers.
                Include(x => x.Club)
                .Include(x => x.User)
                .Where(u => u.UserId == userId).FirstOrDefaultAsync();
            return clubMember.ClubId;
        }

        public async Task<ClubMember> GetClubMemberByIdAsync(int id)
        {
            var clubMember = await _context.ClubMembers
                .Include(x => x.Club)
                .Include(x => x.User)
                .Where(u => u.ClubMemberId == id).FirstOrDefaultAsync();
            return clubMember;
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<ClubMember> UpdateClubMemberAsync(ClubMember clubMember)
        {
            _context.ClubMembers.Update(clubMember);
            return clubMember;
        }
    }
}
