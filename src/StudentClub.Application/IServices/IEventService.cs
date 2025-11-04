using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;

namespace StudentClub.Application.IServices
{
    public interface IEventService
    {
        Task<CreateEventResponseDto> CreateEventAsync(CreateEventRequestDto request, int userId, string role);
        Task<CreateEventResponseDto> UpdateEventAsync(UpdateEventRequestDto requestDto, int eventId, int userId, string role);
        Task<List<GetAllEventsResponseDto>> GetAllEventsAsync(string role, int userId);
        Task<GetAllEventsResponseDto> GetEventByIdAsync(int eventId);
        Task<List<GetAllEventsResponseDto>> GetPublicEventsAsync();
        Task<List<GetAllEventsResponseDto>> GetPublicEventsByClubIdAsync(int clubId);
        Task<List<GetAllEventsResponseDto>> GetEventsByClubIdAsync(int userId);
        Task DeleteEvent(int id);
        Task<List<GetAllEventsResponseDto>> GetEventsByClubIdAsync(int clubId, string role);
    }
}
