using StudentClub.Domain.Entities;
using System.Threading.Tasks;

namespace StudentClub.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
    }
}
