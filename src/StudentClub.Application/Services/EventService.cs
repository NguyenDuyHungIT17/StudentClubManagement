using MediatR;
using StudentClub.Application.DTOs;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentClub.Application.Mapper;

namespace StudentClub.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IClubMemberRepository _clubmemberRepository;
        private readonly IUserRepository _userRepository;
        private readonly EventMapping _eventMapper;

        public EventService(IEventRepository eventRepository, IClubRepository clubRepository, IClubMemberRepository clubMemberRepository, EventMapping eventMapper, IUserRepository userRepository)
        {
            _eventRepository = eventRepository;
            _clubRepository = clubRepository;
            _eventMapper = eventMapper;
            _userRepository = userRepository;
            _clubmemberRepository = clubMemberRepository;
        }
        public async Task<CreateEventResponseDto> CreateEventAsync(CreateEventRequestDto request, int userId, string role)
        {
            if (role == "leader")
            {
                var club = await _clubRepository.GetClubByClubIdAsync(request.ClubId);
                if (club == null)
                    throw new KeyNotFoundException("Câu lạc bộ không tồn tại");

                if (club.LeaderId != userId)
                    throw new UnauthorizedAccessException("Bạn không có quyền truy cập");
            }

            var ev = new Event
            {
                EventDate = request.EventDate,
                ClubId = request.ClubId,
                Description = request.Description,
                Title = request.Title,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsPrivate = request.IsPrivate,
            };

            await _eventRepository.AddEventAsync(ev);
            await _eventRepository.SaveChangeAsync();

            var evDto = new CreateEventResponseDto
            {
                ClubName = await _clubRepository.GetCLubNameByClubIdAsync(request.ClubId),
                Description = ev.Description,
                Title = ev.Title,
                EventDate = ev.EventDate,
            };

            return evDto;
        }

        public async Task<List<GetAllEventsResponseDto>> GetAllEventsAsync(string role, int userId)
        {
            var evDto = new List<GetAllEventsResponseDto>();
            if (role == "admin")
            {
               var ev = await _eventRepository.GetAllEventsAsync();
               evDto = await _eventMapper.ToDtoList(ev);
            }else if (role == "leader" || role == "member")
            {
                var clubId = await _clubmemberRepository.GetClubIdByUserId(userId);
                var ev = await _eventRepository.GetEventsByCLubIdAsync(clubId);
                return await _eventMapper.ToDtoList(ev);
            }
            return evDto;
        }

        public async Task<GetAllEventsResponseDto> GetEventByIdAsync(int eventId)
        {
            var ev = await _eventRepository.GetByEventIdAsync(eventId);
            if (ev == null)
                throw new KeyNotFoundException("Sự kiện không tồn tại");

            var evDto = await _eventMapper.ToDto(ev);

            return evDto;
        }

        public async Task<List<GetAllEventsResponseDto>> GetEventsByClubIdAsync(int clubId, string role)
        {
            var evDto = new List<GetAllEventsResponseDto>();
            if (role == "admin")
            {
                var ev = await  _eventRepository.GetEventsByCLubIdAsync(clubId);
                evDto = await _eventMapper.ToDtoList(ev);
            }
            else if (role == "leader" || role == "member")
            {
                var ev = await _eventRepository.GetEventsByCLubIdAsync(clubId);
                evDto = await _eventMapper.ToDtoList(ev);
            }

            return evDto;
        }

        public async Task<List<GetAllEventsResponseDto>> GetPublicEventsAsync()
        {
            var ev = await  _eventRepository.GetPublicEventsAsync(false);
            var evDto = new List<GetAllEventsResponseDto>();
                evDto = await  _eventMapper.ToDtoList(ev);
            if (evDto.Count == 0)
                throw new KeyNotFoundException("Không có sự kiện công khai nào");
            return evDto;
        }

        public async Task<List<GetAllEventsResponseDto>> GetPublicEventsByClubIdAsync(int clubId)
        {
            var ev = await _eventRepository.GetPublicEventsByCLubIdAsync(clubId, false);
            var evDto = new List<GetAllEventsResponseDto>();
            evDto = await _eventMapper.ToDtoList(ev);
            if (evDto.Count == 0)
                throw new KeyNotFoundException("Không có sự kiện công khai nào");

            return evDto;
        }

        public async Task<CreateEventResponseDto> UpdateEventAsync(UpdateEventRequestDto requestDto,int eventId, int userId, string role)
        {
            if (role == "leader")
            {
                var club = await _clubRepository.GetClubByClubIdAsync(requestDto.ClubId);
                if (club == null)
                    throw new KeyNotFoundException("Câu lạc bộ không tồn tại");

                if (club.LeaderId != userId)
                    throw new UnauthorizedAccessException("Bạn không có quyền truy cập");
            }

            var ev = await _eventRepository.GetByEventIdAsync(eventId);
            if (ev == null)
            {
                throw new KeyNotFoundException("Sự kiện không tồn tại");
            }

            ev.EventDate = requestDto.EventDate;
            ev.Title = requestDto.Title;
            ev.UpdatedAt = DateTime.UtcNow;
            ev.Description = requestDto.Description;
            ev.ClubId = requestDto.ClubId;
            ev.IsPrivate = requestDto.IsPrivate;

            await _eventRepository.SaveChangeAsync();

            var evDto = new CreateEventResponseDto
            {
                ClubName = await _clubRepository.GetCLubNameByClubIdAsync(requestDto.ClubId),
                Description = ev.Description,
                Title = ev.Title,
                EventDate = ev.EventDate,
            };

            return evDto;
        }
    }
}
