using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.IServices
{
    public interface IEventRegistrationService
    {
        Task<ApiResponse<CreateEventRegistrationResponseDto>> CreateEventRegistrationAsync(CreateEventRegistrationRequestDto request, int userId);
        Task<ApiResponse> DeleteEventRegistration(int eventRegistrationId, string role, int userId);
        Task<ApiResponse<List<CreateEventRegistrationResponseDto>>> GetAllEventRegistrationsByEventId(int eventId);
    }
}