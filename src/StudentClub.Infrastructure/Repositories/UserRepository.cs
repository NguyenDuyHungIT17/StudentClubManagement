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

        public async Task AddAsync(User user)
        {
             await _context.AddAsync(user);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByFullnameAsync(string fullname)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.FullName == fullname);
        }

        public Task<User> GetUserByUserIdAsync(int userId)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task SaveChangeAsynce()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
        }

        public async Task DeleteUserAsync(User user)
        {
            var feedbacks = _context.Feedbacks.Where(f => f.UserId == user.UserId);
            var registration = _context.EventRegistrations.Where(r => r.UserId == user.UserId);
            var clubMembers = _context.ClubMembers.Where(c => c.UserId == user.UserId);
            var clubsLeader = _context.Clubs.Where( cl => cl.LeaderId == user.UserId);

            _context.Feedbacks.RemoveRange(feedbacks);
            _context.EventRegistrations.RemoveRange(registration);
            _context.ClubMembers.RemoveRange(clubMembers);

            await clubsLeader.ForEachAsync(cl => cl.LeaderId = null);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetIsActiveByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return 0;
            return user.IsActive == 1 ? 1 : 0;
        }
    }
}
