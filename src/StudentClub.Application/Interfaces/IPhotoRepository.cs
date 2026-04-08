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
        Task<List<Photo>> GetPhotosByCampaignIdAsync(int campaignId);

        // NEW: main-photo helpers
        Task<Photo?> GetMainPhotoAsync(int? userId, int? clubId, int? eventId, int? clubMemberId, int? campaignId);
        Task<Dictionary<int, Photo?>> GetMainPhotosByUserIdsAsync(List<int> userIds);
        Task<Dictionary<int, Photo?>> GetMainPhotosByClubIdsAsync(List<int> clubIds);
        Task<Dictionary<int, Photo?>> GetMainPhotosByEventIdsAsync(List<int> eventIds);
        Task<Dictionary<int, Photo?>> GetMainPhotosByClubMemberIdsAsync(List<int> clubMemberIds);
        Task<Dictionary<int, Photo?>> GetMainPhotosByCampaignIdsAsync(List<int> campaignIds);

        Task UpdatePhotoAsync(Photo photo);
        Task DeletePhotoAsync(Photo photo);

        Task DeletePhotosByUserIdAsync(int userId);
        Task DeletePhotosByClubIdAsync(int clubId);
        Task DeletePhotosByEventIdAsync(int eventId);
        Task DeletePhotosByClubMemberIdAsync(int clubMemberId);
        Task DeletePhotosByCampaignIdAsync(int campaignId);

        Task DeletePhotoByAnyway(int anyId, int type); // type: 1-user, 2-club, 3-event, 4-clubMember, 5-campaign
        Task SaveChangesAsync();
    }
}