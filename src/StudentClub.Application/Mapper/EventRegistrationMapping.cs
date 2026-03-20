using StudentClub.Application.DTOs.request.EventRegistration;
using StudentClub.Application.DTOs.response.EventRegistration;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;

namespace StudentClub.Application.Mapper
{
    public class EventRegistrationMapping
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        public EventRegistrationMapping(IEventRepository eventRepository, IUserRepository userRepository)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
        }

        public async virtual Task<CreateEventRegistrationResponseDto> MapToCreateEventRegistrationResponseDto(EventRegistration eventRegistration)
        {
            return new CreateEventRegistrationResponseDto
            {
                Id = eventRegistration.RegistrationId,
                EventId = eventRegistration.EventId,
                UserId = eventRegistration.UserId,
                CheckedIn = eventRegistration.CheckedIn,
                CheckName = eventRegistration.CheckName,
                GuestEmail = eventRegistration.GuestEmail,
                GuestName = eventRegistration.GuestName,
                IsCare = eventRegistration.IsCare,
                RegisteredAt = eventRegistration.RegisteredAt,
                CreatedAt = eventRegistration.CreatedAt,
                UpdatedAt = eventRegistration.UpdatedAt,
                CLubId = await _eventRepository.GetClubIdByEventId(eventRegistration.EventId),
            };
        }

        public async virtual Task<List<CreateEventRegistrationResponseDto>> MapToDtoList(List<EventRegistration> ev)
        {
            var result = new List<CreateEventRegistrationResponseDto>();
            foreach (var item in ev)
            {
                var dto = await MapToCreateEventRegistrationResponseDto(item);
                result.Add(dto);
            }
            return result;
        }

        public async virtual Task<EventRegistration> MapToEntity(CreateEventRegistrationRequestDto ev)
        {
            return new EventRegistration
            {
                EventId = ev.EventId,
                UserId = ev.UserId,
                CheckedIn = ev.CheckedIn,
                CheckName = ev.CheckName,
                RegisteredAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                GuestName = ev.GuestName,
                GuestEmail = ev.GuestEmail,
                IsCare= ev.IsCare,
            };
        }
    }
}
