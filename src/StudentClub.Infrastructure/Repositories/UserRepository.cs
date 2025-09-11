using Microsoft.EntityFrameworkCore;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using StudentClub.Infrastructure.Persistence;
using System.Threading.Tasks;

namespace StudentClub.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly StudentClubDbContext _context;

        public UserRepository(StudentClubDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
