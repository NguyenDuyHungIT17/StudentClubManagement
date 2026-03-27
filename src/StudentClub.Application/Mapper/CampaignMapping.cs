using StudentClub.Application.DTOs.request.Campaign;
using StudentClub.Application.DTOs.response.Campaign;
using StudentClub.Domain.Entities;

namespace StudentClub.Application.Mapper
{
    public class CampaignMapping
    {
        public static CampaignResponse ToDto(Campaigns campaign)
        {
            return new CampaignResponse
            {
                CampaignId = campaign.CampaignId,
                ClubId = campaign.ClubId,
                Title = campaign.Title,
                StartDate = campaign.StartDate,
                EndDate = campaign.EndDate,
                IsActive = campaign.IsActive,
                CreatedAt = campaign.CreatedAt
            };
        }

        public static List<CampaignResponse> ToDtoList(List<Campaigns> campaigns)
        {
            return campaigns.Select(ToDto).ToList();
        }

        public static Campaigns ToEntity(CampaignRequest dto)
        {
            return new Campaigns
            {
                ClubId = dto.ClubId,
                Title = dto.Title,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static Campaigns UpdateEntity(Campaigns campaign, CampaignRequest dto)
        {
            campaign.Title = dto.Title;
            campaign.StartDate = dto.StartDate;
            campaign.EndDate = dto.EndDate;
            campaign.IsActive = dto.IsActive;
            return campaign;
        }
    }
}