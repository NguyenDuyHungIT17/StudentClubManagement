using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Shared.ApiResponse; // Thêm namespace này

namespace StudentClub.Application.IServices
{
    public interface IInterviewService
    {
        Task<ApiResponse<GetInterviewResponseDto>> CreateAsync(CreateInterviewRequestDto request, int userId, string role);
        Task<ApiResponse<GetInterviewResponseDto>> CreateWebAsync(CreateInterviewRequestDto request);
        Task<ApiResponse<GetInterviewResponseDto>> UpdateAsync(int id, UpdateInterviewRequestDto request, int userId, string role);
        Task<ApiResponse<GetInterviewResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse> DeleteAsync(int id, int userId, string role);
        Task<ApiResponse<List<GetInterviewResponseDto>>> GetByClubIdAsync(int clubId, int userId, string role);
        Task<ApiResponse<List<GetInterviewResponseDto>>> GetAllAsync();
    }
}