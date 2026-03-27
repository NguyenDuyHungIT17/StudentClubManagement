using StudentClub.Domain.Entities;

namespace StudentClub.Application.Interfaces
{
    public interface ICampaignRepository
    {
        // Create
        Task AddCampaignAsync(Campaigns campaign);

        // Read
        Task<Campaigns?> GetCampaignByIdAsync(int campaignId);
        Task<List<Campaigns>> GetAllCampaignsAsync();
        Task<List<Campaigns>> GetCampaignsByClubIdAsync(int clubId);

        // Update
        Task UpdateCampaignAsync(Campaigns campaign);

        // Delete
        Task DeleteCampaignAsync(Campaigns campaign);

        // Save
        Task SaveChangeAsync();
    }
}