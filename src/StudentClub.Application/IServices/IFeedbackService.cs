using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;

namespace StudentClub.Application.IServices
{
    public interface IFeedbackService
    {
        Task<CreateFeedbackResponseDto> CreateFeedbackAsync(CreateFeedbackRequestDto feedbackDto, int userIdOnToken);
        Task<CreateFeedbackResponseDto> GetFeedbackByIdAsync(int feedbackId);
        Task<List<CreateFeedbackResponseDto>> GetAllFeedbacksAsync();
        Task<List<CreateFeedbackResponseDto>> GetFeedbacksByEventIdAsync(int eventId);
        Task<CreateFeedbackResponseDto> UpdateFeedbackAsync(int id, CreateFeedbackRequestDto feedbackDto);
        Task DeleteFeedbackAsync(int feedbackId);
    }
}
