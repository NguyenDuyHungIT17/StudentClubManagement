using Microsoft.EntityFrameworkCore;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using StudentClub.Infrastructure.Persistence;

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

        public IQueryable<User> QueryUsers()
        {
            return _context.Users.AsNoTracking();
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
            // Soft delete user: chỉ set IsActive = 0
            user.IsActive = 0;

            // Nếu user đang là leader của CLB nào đó thì bỏ leader
            var clubsLeader = await _context.Clubs
                .Where(cl => cl.LeaderId == user.UserId)
                .ToListAsync();

            foreach (var club in clubsLeader)
            {
                club.LeaderId = null;
            }

            // Cập nhật user & clubs
            _context.Users.Update(user);
            _context.Clubs.UpdateRange(clubsLeader);

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetIsActiveByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return 0;
            return user.IsActive == 1 ? 1 : 0;
        }

        public async Task<List<User>?> GetAllUsersAsync()
        {      
                return await _context.Users.ToListAsync();
        }

        public async Task<List<User>> GetUserByLeader(int? clubId)
        {
            var users = await (from cm in _context.ClubMembers
                               join u in _context.Users on cm.UserId equals u.UserId
                               where cm.ClubId == clubId && cm.MemberRole == "member"
                               select u).ToListAsync();

            return users;
        }

        public async Task<string?> GetEmailByUserIdAsync(int userId)
        {
            var email = await _context.Users.Where(u => u.UserId == userId).Select(u => u.Email).FirstOrDefaultAsync();
            return email;
        }

        public async Task UpdatePasswordAsync(int userId, string newPasswordHash)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new KeyNotFoundException("User not found");
            user.PasswordHash = newPasswordHash;
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetUserNameByIdAsync(int userId)
        {
            var user = await  _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return user.FullName;
        }
    }
}
