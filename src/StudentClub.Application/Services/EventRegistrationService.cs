using StudentClub.Application.DTOs;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;
using StudentClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.Services
{
    public class EventRegistrationService : IEventRegistrationService
    {
        private readonly IEventRegistrationRepository _eventRegistrationRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IEventRepository _eventRepository;   
        private readonly EventRegistrationMapping _eventMapping;

        public EventRegistrationService(IEventRegistrationRepository eventRegistrationRepository, EventRegistrationMapping eventMapping, IClubRepository clubRepository, IEventRepository eventRepository)
        {
            _eventRegistrationRepository = eventRegistrationRepository;
            _eventMapping = eventMapping;
            _clubRepository = clubRepository;
            _eventRepository = eventRepository;
        }
        public async Task<CreateEventRegistrationResponseDto> CreateEventRegistrationAsync(CreateEventRegistrationRequestDto request)
        {
            var response = new EventRegistration();
            if (request == null)
            {
                throw new KeyNotFoundException("yêu cầu không thỏa mãn");
            }

            response.EventId = request.EventId;
            response.UserId = request.UserId;
            response.CheckedIn = request.CheckedIn;
            response.RegisteredAt = DateTime.Now;
            response.CreatedAt = DateTime.Now;
            response.UpdatedAt = DateTime.Now;

            await _eventRegistrationRepository.AddEventRegistrationAsync(response);
            await _eventRegistrationRepository.SaveChangeAsynce();

            var responseDto = await _eventMapping.MapToCreateEventRegistrationResponseDto(response);

            return responseDto;
        }

        public async Task DeleteEventRegistration(int eventRegistrationId, string role, int userId)
        {
            var er = await _eventRegistrationRepository.GetEventRegistrationByIdAsync(eventRegistrationId);

            if (er == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thông tin");
            }

            var ev = await _eventRepository.GetEventByIdAsync(er.EventId);

            if (ev == null)
            {
                throw new KeyNotFoundException("Sự kiện không tồn tại");
            }

            if (role == "leader")
            {
                var club = await _clubRepository.GetClubByClubIdAsync(ev.ClubId);
                if (club == null)
                    throw new KeyNotFoundException("Câu lạc bộ không tồn tại");

                if (club.LeaderId != userId)
                    throw new UnauthorizedAccessException("Bạn không có quyền xóa sự kiện này");

                await _eventRegistrationRepository.DeleteEventRegistrationAsync(eventRegistrationId);
            }

            if (role == "admin")
            {
                await _eventRegistrationRepository.DeleteEventRegistrationAsync(eventRegistrationId);
            }
        }

        public async Task<List<CreateEventRegistrationResponseDto>> GetAllEventRegistrationsByEventId(int eventId)
        {
            var ers = await _eventRegistrationRepository.GetEventRegistrationByEventIdAsync(eventId);
            if (ers == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thông tin");
            }
            var ersDto = new List<CreateEventRegistrationResponseDto>();
            ersDto = await _eventMapping.MapToDtoList(ers);
            return ersDto;
        }
    }
}
