using StudentClub.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentClub.Application.Interfaces
{
    public interface IInterviewRepository
    {
        Task AddAsync(Interview interview);
        Task<Interview?> GetByIdAsync(int id);
        Task<Interview?> GetByClubIdAndEmail(int clubId,string email);
        Task<List<Interview>> GetByClubIdAsync(int clubId);
        Task<List<Interview>> GetAllAsync();
        Task DeleteAsync(Interview interview);
        Task SaveChangesAsync();
    }
}
