using Org.BouncyCastle.Asn1.Ocsp;
using StudentClub.Application.DTOs;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.Mapper
{
    public class EventMapping
    {
        private readonly IClubRepository _clubRepository;
        public EventMapping(IClubRepository clubRepository)
        {
            _clubRepository = clubRepository;
        }
        public async virtual Task<GetAllEventsResponseDto> ToDto(Event ev)
        {
            return new GetAllEventsResponseDto
            {
                ClubName = await _clubRepository.GetCLubNameByClubIdAsync(ev.ClubId),
                Description = ev.Description,
                Title = ev.Title,
                EventDate = ev.EventDate,
            };
        }
        public async virtual Task<List<GetAllEventsResponseDto>> ToDtoList(List<Event> ev)
        {
            var result = new List<GetAllEventsResponseDto>();
            foreach (var item in ev)
            {
                var dto = await ToDto(item);
                result.Add(dto);
            }
            return result;
        }
    }
}
