using Microsoft.EntityFrameworkCore;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using StudentClub.Infrastructure.Persistence;

namespace StudentClub.Infrastructure.Repositories
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly StudentClubDbContext _context;

        public CampaignRepository(StudentClubDbContext context)
        {
            _context = context;
        }

        public async Task AddCampaignAsync(Campaigns campaign)
        {
            await _context.Campaigns.AddAsync(campaign);
        }

        public async Task<Campaigns?> GetCampaignByIdAsync(int campaignId)
        {
            return await _context.Campaigns
                .FirstOrDefaultAsync(c => c.CampaignId == campaignId);
        }

        public async Task<List<Campaigns>> GetAllCampaignsAsync()
        {
            return await _context.Campaigns
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Campaigns>> GetCampaignsByClubIdAsync(int clubId)
        {
            return await _context.Campaigns
                .Where(c => c.ClubId == clubId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateCampaignAsync(Campaigns campaign)
        {
            _context.Campaigns.Update(campaign);
            await Task.CompletedTask;
        }

        public async Task DeleteCampaignAsync(Campaigns campaign)
        {
            _context.Campaigns.Remove(campaign);
            await Task.CompletedTask;
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}