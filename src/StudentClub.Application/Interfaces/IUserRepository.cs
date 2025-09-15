using StudentClub.Domain.Entities;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace StudentClub.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<int> GetIsActiveByEmailAsync(string email);
        Task<User> GetUserByUserIdAsync(int userId);
        Task<User> GetByFullnameAsync(string username);
        Task AddAsync(User user);

        Task SaveChangeAsynce();
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
    }
}
