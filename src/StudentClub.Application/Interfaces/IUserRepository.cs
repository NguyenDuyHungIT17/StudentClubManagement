using StudentClub.Domain.Entities;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace StudentClub.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<int> GetIsActiveByEmailAsync(string email);
        Task<string?> GetEmailByUserIdAsync(int userId);
        Task<User> GetUserByUserIdAsync(int userId);
        Task<User> GetByFullnameAsync(string username);
        Task<List<User>> GetUserByLeader(int clubId);
        Task<List<User>?> GetAllUsersAsync();
        Task AddAsync(User user);

        Task SaveChangeAsynce();
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);

        Task UpdatePasswordAsync(int userId, string newPasswordHash);
    }
}
