using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.Campaign;
using StudentClub.Application.DTOs.response.Campaign;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;
using StudentClub.Domain.Entities;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(
            ICampaignRepository campaignRepository,
            IClubRepository clubRepository,
            IPhotoService photoService,
            ILogger<CampaignService> logger)
        {
            _campaignRepository = campaignRepository;
            _clubRepository = clubRepository;
            _photoService = photoService;
            _logger = logger;
        }

        public async Task<ApiResponse<CampaignResponse>> CreateCampaignAsync(CampaignRequest request)
        {
            try
            {
                // Verify club exists
                var club = await _clubRepository.GetClubByClubIdAsync(request.ClubId);
                if (club == null)
                {
                    return ApiResponse<CampaignResponse>.Failure(404, "Câu lạc bộ không tồn tại");
                }

                var campaign = CampaignMapping.ToEntity(request);
                await _campaignRepository.AddCampaignAsync(campaign);
                await _campaignRepository.SaveChangeAsync();

                var result = CampaignMapping.ToDto(campaign);
                return ApiResponse<CampaignResponse>.Success(result, "Tạo chiến dịch tuyển dụng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo chiến dịch tuyển dụng. Tiêu đề: {Title}, Thời gian: {Time}", request.Title, DateTime.UtcNow);
                return ApiResponse<CampaignResponse>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<CampaignResponse>> GetCampaignByIdAsync(int campaignId)
        {
            try
            {
                var campaign = await _campaignRepository.GetCampaignByIdAsync(campaignId);
                if (campaign == null)
                {
                    return ApiResponse<CampaignResponse>.Failure(404, "Chiến dịch tuyển dụng không tồn tại");
                }

                var result = CampaignMapping.ToDto(campaign);
                // attach main photo url for this campaign
                result.PhotoUrl = await _photoService.GetMainPhotoUrlAsync(null, null, null, null, campaign.CampaignId);
                return ApiResponse<CampaignResponse>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chiến dịch tuyển dụng. CampaignId: {CampaignId}, Thời gian: {Time}", campaignId, DateTime.UtcNow);
                return ApiResponse<CampaignResponse>.Failure(500, ex.Message);
            }
        }

        public async Task<PagedResponse<CampaignResponse>> GetCampaignsAsync(CampaignFilterRequest filter)
        {
            try
            {
                var campaigns = await _campaignRepository.GetAllCampaignsAsync();

                // Apply filters
                if (filter.ClubId.HasValue && filter.ClubId > 0)
                {
                    campaigns = campaigns.Where(c => c.ClubId == filter.ClubId).ToList();
                }

                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    var keyword = filter.KeyWord.Trim().ToLower();
                    campaigns = campaigns.Where(c => c.Title.ToLower().Contains(keyword)).ToList();
                }

                if (filter.IsActive.HasValue)
                {
                    campaigns = campaigns.Where(c => c.IsActive == filter.IsActive).ToList();
                }

                if (filter.FromDate.HasValue)
                {
                    campaigns = campaigns.Where(c => c.StartDate >= filter.FromDate).ToList();
                }

                if (filter.ToDate.HasValue)
                {
                    campaigns = campaigns.Where(c => c.EndDate <= filter.ToDate).ToList();
                }

                // Apply sorting
                campaigns = ApplySorting(campaigns, filter.SortBy, filter.SortDesc);

                // Apply pagination
                var totalCount = campaigns.Count;
                var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
                var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

                var items = campaigns
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => CampaignMapping.ToDto(c))
                    .ToList();

                // Batch fetch main photo URLs for campaigns on this page
                var campaignIds = items.Select(i => i.CampaignId).ToList();
                var photoMap = campaignIds.Count > 0
                    ? await _photoService.GetMainPhotoUrlsByCampaignIdsAsync(campaignIds)
                    : new Dictionary<int, string?>();

                foreach (var dto in items)
                {
                    dto.PhotoUrl = photoMap.ContainsKey(dto.CampaignId) ? photoMap[dto.CampaignId] : null;
                }

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return new PagedResponse<CampaignResponse>
                {
                    Items = items,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách chiến dịch tuyển dụng. Thời gian: {Time}", DateTime.UtcNow);
                throw;
            }
        }

        public async Task<ApiResponse<CampaignResponse>> UpdateCampaignAsync(int campaignId, CampaignRequest request)
        {
            try
            {
                var campaign = await _campaignRepository.GetCampaignByIdAsync(campaignId);
                if (campaign == null)
                {
                    return ApiResponse<CampaignResponse>.Failure(404, "Chiến dịch tuyển dụng không tồn tại");
                }

                // Verify club exists
                var club = await _club_repository_getClubByClubIdAsync(request.ClubId);
                if (club == null)
                {
                    return ApiResponse<CampaignResponse>.Failure(404, "Câu lạc bộ không tồn tại");
                }

                campaign = CampaignMapping.UpdateEntity(campaign, request);
                await _campaignRepository.UpdateCampaignAsync(campaign);
                await _campaignRepository.SaveChangeAsync();

                var result = CampaignMapping.ToDto(campaign);
                // attach photo
                result.PhotoUrl = await _photoService.GetMainPhotoUrlAsync(null, null, null, null, campaign.CampaignId);
                return ApiResponse<CampaignResponse>.Success(result, "Cập nhật chiến dịch tuyển dụng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật chiến dịch tuyển dụng. CampaignId: {CampaignId}, Thời gian: {Time}", campaignId, DateTime.UtcNow);
                return ApiResponse<CampaignResponse>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteCampaignAsync(int campaignId)
        {
            try
            {
                var campaign = await _campaignRepository.GetCampaignByIdAsync(campaignId);
                if (campaign == null)
                {
                    return ApiResponse.Failure(404, "Chiến dịch tuyển dụng không tồn tại");
                }

                await _campaignRepository.DeleteCampaignAsync(campaign);
                await _campaign_repository_saveChangeAsync();

                return ApiResponse.Success("Xóa chiến dịch tuyển dụng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa chiến dịch tuyển dụng. CampaignId: {CampaignId}, Thời gian: {Time}", campaignId, DateTime.UtcNow);
                return ApiResponse.Failure(500, ex.Message);
            }
        }

        private List<Campaigns> ApplySorting(List<Campaigns> campaigns, string? sortBy, bool sortDesc)
        {
            return (sortBy?.ToLower()) switch
            {
                "title" => sortDesc
                    ? campaigns.OrderByDescending(c => c.Title).ToList()
                    : campaigns.OrderBy(c => c.Title).ToList(),
                "startdate" => sortDesc
                    ? campaigns.OrderByDescending(c => c.StartDate).ToList()
                    : campaigns.OrderBy(c => c.StartDate).ToList(),
                "enddate" => sortDesc
                    ? campaigns.OrderByDescending(c => c.EndDate).ToList()
                    : campaigns.OrderBy(c => c.EndDate).ToList(),
                "isactive" => sortDesc
                    ? campaigns.OrderByDescending(c => c.IsActive).ToList()
                    : campaigns.OrderBy(c => c.IsActive).ToList(),
                _ => sortDesc
                    ? campaigns.OrderByDescending(c => c.CreatedAt).ToList()
                    : campaigns.OrderBy(c => c.CreatedAt).ToList(),
            };
        }

        // small helpers to avoid many DI changes in other files
        private Task<Club?> _club_repository_getClubByClubIdAsync(int id) => _clubRepository.GetClubByClubIdAsync(id);
        private Task _campaign_repository_saveChangeAsync() => _campaignRepository.SaveChangeAsync();
    }
}