using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.IServices
{
    public interface IFeedbackService
    {
        Task<ApiResponse<CreateFeedbackResponseDto>> CreateFeedbackAsync(CreateFeedbackRequestDto feedbackDto, int userIdOnToken);
        Task<ApiResponse<CreateFeedbackResponseDto>> GetFeedbackByIdAsync(int feedbackId);
        Task<ApiResponse<List<CreateFeedbackResponseDto>>> GetAllFeedbacksAsync();
        Task<ApiResponse<List<CreateFeedbackResponseDto>>> GetFeedbacksByEventIdAsync(int eventId);
        Task<ApiResponse<CreateFeedbackResponseDto>> UpdateFeedbackAsync(int id, CreateFeedbackRequestDto feedbackDto);
        Task<ApiResponse> DeleteFeedbackAsync(int feedbackId);
    }
}