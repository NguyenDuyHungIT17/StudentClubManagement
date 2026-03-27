using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.Campaign;
using StudentClub.Application.DTOs.response.Campaign;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.IServices
{
    public interface ICampaignService
    {
        Task<ApiResponse<CampaignResponse>> CreateCampaignAsync(CampaignRequest request);
        Task<ApiResponse<CampaignResponse>> GetCampaignByIdAsync(int campaignId);
        Task<PagedResponse<CampaignResponse>> GetCampaignsAsync(CampaignFilterRequest filter);
        Task<ApiResponse<CampaignResponse>> UpdateCampaignAsync(int campaignId, CampaignRequest request);
        Task<ApiResponse> DeleteCampaignAsync(int campaignId);
    }
}