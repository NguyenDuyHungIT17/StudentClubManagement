using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.EventRegistration;
using StudentClub.Application.DTOs.response.EventRegistration;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.IServices
{
    public interface IEventRegistrationService
    {
        Task<ApiResponse<CreateEventRegistrationResponseDto>> CreateEventRegistrationWithUserAsync(CreateEventRegistrationRequestDto request, int userId);
        Task<ApiResponse<CreateEventRegistrationResponseDto>> CreateEventRegistrationGuestAsync(CreateEventRegistrationRequestDto request);
        Task<ApiResponse> DeleteEventRegistration(int eventRegistrationId, string role, int userId);
        Task<PagedResponse<CreateEventRegistrationResponseDto>> GetAllEventRegistrationsByEventId(int eventId, EventRegistrationFilter filter);
        Task<ApiResponse<CreateEventRegistrationResponseDto>> Update(int id, CreateEventRegistrationRequestDto request, string role, int userId);
        Task<ApiResponse<CreateEventRegistrationResponseDto>> GetById(int id);
    }
}