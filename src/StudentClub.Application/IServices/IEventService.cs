using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.Event;
using StudentClub.Application.DTOs.response.Event;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.IServices
{
    public interface IEventService
    {
        Task<ApiResponse<CreateEventResponseDto>> CreateEventAsync(CreateEventRequestDto request, int userId, string role);
        Task<ApiResponse<CreateEventResponseDto>> UpdateEventAsync(UpdateEventRequestDto requestDto, int eventId, int userId, string role);
        Task<PagedResponse<CreateEventResponseDto>> GetAllEventsAsync(EventFilterRequest filter, string role, int userId);
        Task<ApiResponse<CreateEventResponseDto>> GetEventByIdAsync(int eventId);
        Task<ApiResponse<List<CreateEventResponseDto>>> GetPublicEventsAsync();
        Task<ApiResponse<List<CreateEventResponseDto>>> GetPublicEventsByClubIdAsync(int clubId);
        Task<ApiResponse<List<CreateEventResponseDto>>> GetEventsByClubIdAsync(int userId);
        Task<ApiResponse> DeleteEvent(int id);
        Task<ApiResponse<List<CreateEventResponseDto>>> GetEventsByClubIdAsync(int clubId, string role);
    }
}