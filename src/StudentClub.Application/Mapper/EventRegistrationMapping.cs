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
                EventName = await _eventRepository.GetEventNameByIdAsync(eventRegistration.EventId),
                UserName = eventRegistration.CheckName,
                CheckedIn = eventRegistration.CheckedIn,
                RegisteredAt = eventRegistration.RegisteredAt,
                EventDate = eventRegistration.Event.EventDate
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
            };
        }
    }
}
