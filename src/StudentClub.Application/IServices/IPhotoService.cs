using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.IServices
{
    public interface IPhotoService
    {
        Task<ApiResponse<PhotoResponseDto>> UploadPhotoAsync(UploadPhotoRequestDto request);
        Task<ApiResponse> DeletePhotoAsync(int photoId);
        Task<ApiResponse<List<PhotoResponseDto>>> GetPhotosByEventIdAsync(int eventId);
        Task<ApiResponse<List<PhotoResponseDto>>> GetPhotosByClubIdAsync(int clubId);
        Task<ApiResponse<List<PhotoResponseDto>>> GetPhotosByUserIdAsync(int userId);
        Task<ApiResponse<PhotoResponseDto>> UpdatePhotoAsync(int photoId, UpdatePhotoRequestDto request, int userId);
        Task<ApiResponse<List<PhotoResponseDto>>> GetPhotosByClubMemberIdAsync(int clubMemberId);
        Task<ApiResponse<List<PhotoResponseDto>>> GetPhotosByCampaignIdAsync(int campaignId);

        // NEW: helpers to return main photo URL(s)
        Task<string?> GetMainPhotoUrlAsync(int? userId = null, int? clubId = null, int? eventId = null, int? clubMemberId = null, int? campaignId = null);

        // Batch helpers: return map id -> url (null if none)
        Task<Dictionary<int, string?>> GetMainPhotoUrlsByUserIdsAsync(List<int> userIds);
        Task<Dictionary<int, string?>> GetMainPhotoUrlsByClubIdsAsync(List<int> clubIds);
        Task<Dictionary<int, string?>> GetMainPhotoUrlsByEventIdsAsync(List<int> eventIds);
        Task<Dictionary<int, string?>> GetMainPhotoUrlsByClubMemberIdsAsync(List<int> clubMemberIds);
        Task<Dictionary<int, string?>> GetMainPhotoUrlsByCampaignIdsAsync(List<int> campaignIds);
    }
}