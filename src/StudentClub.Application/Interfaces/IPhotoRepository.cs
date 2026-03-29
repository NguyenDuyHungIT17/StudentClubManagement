using StudentClub.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentClub.Application.Interfaces
{
    public interface IPhotoRepository
    {
        Task AddPhotoAsync(Photo photo);
        Task AddRangeAsync(IEnumerable<Photo> photos); 
        Task<Photo?> GetPhotoByIdAsync(int photoId);
        Task<List<Photo>> GetPhotosByEventIdAsync(int eventId);
        Task<List<Photo>> GetPhotosByClubIdAsync(int clubId);
        Task<List<Photo>> GetPhotosByUserIdAsync(int userId);
        Task<List<Photo>> GetPhotosByClubMemberIdAsync(int clubMemberId);
        Task UpdatePhotoAsync(Photo photo);
        Task DeletePhotoAsync(Photo photo);
        Task SaveChangesAsync();
    }
}