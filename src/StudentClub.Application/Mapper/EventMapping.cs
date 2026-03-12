using Org.BouncyCastle.Asn1.Mozilla;
using StudentClub.Application.DTOs.request.Event;
using StudentClub.Application.DTOs.response.Event;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using StudentClub.Domain.Enums;

namespace StudentClub.Application.Mapper
{
    public class EventMapping
    {
        private readonly IClubRepository _clubRepository;
        public EventMapping(IClubRepository clubRepository)
        {
            _clubRepository = clubRepository;
        }
        public async virtual Task<CreateEventResponseDto> ToDto(Event ev)
        {
            return new CreateEventResponseDto
            {
                ClubId = ev.ClubId,
                IsPrivate = ev.IsPrivate,
                Description = ev.Description,
                Title = ev.Title,
                EventDate = ev.EventDate,
                Id = ev.EventId,
                Priority = (int?)ev.Priority,
            };
        }
        public async virtual Task<List<CreateEventResponseDto>> ToDtoList(List<Event> ev)
        {
            var result = new List<CreateEventResponseDto>();
            foreach (var item in ev)
            {
                var dto = await ToDto(item);
                result.Add(dto);
            }
            return result;
        }

        public async virtual Task<Event> ToEntity(CreateEventRequestDto dto)
        {
            return new Event
            {
                ClubId = dto.ClubId,
                IsPrivate = dto.IsPrivate,
                Description = dto.Description,
                Title = dto.Title,
                EventDate = dto.EventDate,
                CreatedAt = DateTime.UtcNow,
                Priority = dto.Priority.HasValue
                            ? (EventPriority)dto.Priority.Value
                            : null
            };
        }
    }
}
