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

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
