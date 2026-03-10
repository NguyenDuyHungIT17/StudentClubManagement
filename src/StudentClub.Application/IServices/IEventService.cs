using StudentClub.Application.DTOs.request.Event;
using StudentClub.Application.DTOs.response.Event;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.IServices
{
    public interface IEventService
    {
        Task<ApiResponse<CreateEventResponseDto>> CreateEventAsync(CreateEventRequestDto request, int userId, string role);
        Task<ApiResponse<CreateEventResponseDto>> UpdateEventAsync(UpdateEventRequestDto requestDto, int eventId, int userId, string role);
        Task<ApiResponse<List<GetAllEventsResponseDto>>> GetAllEventsAsync(string role, int userId);
        Task<ApiResponse<GetAllEventsResponseDto>> GetEventByIdAsync(int eventId);
        Task<ApiResponse<List<GetAllEventsResponseDto>>> GetPublicEventsAsync();
        Task<ApiResponse<List<GetAllEventsResponseDto>>> GetPublicEventsByClubIdAsync(int clubId);
        Task<ApiResponse<List<GetAllEventsResponseDto>>> GetEventsByClubIdAsync(int userId);
        Task<ApiResponse> DeleteEvent(int id);
        Task<ApiResponse<List<GetAllEventsResponseDto>>> GetEventsByClubIdAsync(int clubId, string role);
    }
}