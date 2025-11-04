using Org.BouncyCastle.Crypto.Fpe;
using StudentClub.Application.DTOs.response;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                EventName = await _eventRepository.GetEventNameByIdAsync(eventRegistration.EventId),
                UserName = await _userRepository.GetUserNameByIdAsync(eventRegistration.UserId),
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
    }
}
