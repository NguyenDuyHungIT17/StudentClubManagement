using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;
using StudentClub.Domain.Entities;

namespace StudentClub.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IClubMemberRepository _clubmemberRepository;
        private readonly IUserRepository _userRepository;
        private readonly EventMapping _eventMapper;
        private readonly ILogger<EventService> _logger;

        public EventService(IEventRepository eventRepository, IClubRepository clubRepository, IClubMemberRepository clubMemberRepository, EventMapping eventMapper, IUserRepository userRepository, ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
            _clubRepository = clubRepository;
            _eventMapper = eventMapper;
            _userRepository = userRepository;
            _clubmemberRepository = clubMemberRepository;
            _logger = logger;
        }
        public async Task<CreateEventResponseDto> CreateEventAsync(CreateEventRequestDto request, int userId, string role)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo sự kiện. Tên sự kiện: {EventName}, Thời gian: {Time}", request.Title, DateTime.UtcNow);
                throw;
            }
        }

        public async Task DeleteEvent(int id)
        {
            try
            {
                await _eventRepository.DeleteEvent(id);
                await _clubRepository.SaveChangeAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "lỗi khi xóa sự kiện này");
                throw;
            }
        }

        public async Task<List<GetAllEventsResponseDto>> GetAllEventsAsync(string role, int userId)
        {
            try
            {
                var evDto = new List<GetAllEventsResponseDto>();
                if (role == "admin")
                {
                    var ev = await _eventRepository.GetAllEventsAsync();
                    evDto = await _eventMapper.ToDtoList(ev);
                }
                else if (role == "leader" || role == "member")
                {
                    var clubId = await _clubmemberRepository.GetClubIdByUserId(userId);
                    var ev = await _eventRepository.GetEventsByCLubIdAsync(clubId);
                    return await _eventMapper.ToDtoList(ev);
                }
                return evDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tất cả sự kiện. UserId: {UserId}, Thời gian: {Time}", userId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<GetAllEventsResponseDto> GetEventByIdAsync(int eventId)
        {
            try
            {
                var ev = await _eventRepository.GetByEventIdAsync(eventId);
                if (ev == null)
                    throw new KeyNotFoundException("Sự kiện không tồn tại");
                var evDto = await _eventMapper.ToDto(ev);
                return evDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sự kiện theo ID. EventId: {EventId}, Thời gian: {Time}", eventId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<List<GetAllEventsResponseDto>> GetEventsByClubIdAsync(int clubId, string role)
        {
            try
            {
                var evDto = new List<GetAllEventsResponseDto>();
                if (role == "admin")
                {
                    var ev = await _eventRepository.GetEventsByCLubIdAsync(clubId);
                    evDto = await _eventMapper.ToDtoList(ev);
                }
                else if (role == "leader" || role == "member")
                {
                    var ev = await _eventRepository.GetEventsByCLubIdAsync(clubId);
                    evDto = await _eventMapper.ToDtoList(ev);
                }

                return evDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sự kiện theo câu lạc bộ. ClubId: {ClubId}, Thời gian: {Time}", clubId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<List<GetAllEventsResponseDto>> GetEventsByClubIdAsync(int userId)
        {
            try
            {
                var clubId = await _clubmemberRepository.GetClubIdByUserId(userId);

                var ev = await _eventRepository.GetAllEventsAsync();
                var evByClubId = ev.Where(e => e.ClubId == clubId).ToList();

                var evDto = new List<GetAllEventsResponseDto>();
                evDto = await _eventMapper.ToDtoList(evByClubId);
                if (evDto.Count == 0)
                    throw new KeyNotFoundException("Không có sự kiện công khai nào");

                return evDto;
            }
            catch (Exception ex)
            {
                _logger.LogError("Không tìm thấy sự kiện công khai nào");
                throw;
            }
        }

        public async Task<List<GetAllEventsResponseDto>> GetPublicEventsAsync()
        {
            try
            {
                var ev = await _eventRepository.GetPublicEventsAsync(false);
                var evDto = new List<GetAllEventsResponseDto>();
                evDto = await _eventMapper.ToDtoList(ev);
                if (evDto.Count == 0)
                    throw new KeyNotFoundException("Không có sự kiện công khai nào");
                return evDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sự kiện công khai. Thời gian: {Time}", DateTime.UtcNow);
                throw;
            }
        }

        public async Task<List<GetAllEventsResponseDto>> GetPublicEventsByClubIdAsync(int clubId)
        {
            try
            {
                var ev = await _eventRepository.GetPublicEventsByCLubIdAsync(clubId, false);
                var evDto = new List<GetAllEventsResponseDto>();
                evDto = await _eventMapper.ToDtoList(ev);
                if (evDto.Count == 0)
                    throw new KeyNotFoundException("Không có sự kiện công khai nào");

                return evDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sự kiện công khai theo câu lạc bộ. ClubId: {ClubId}, Thời gian: {Time}", clubId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<CreateEventResponseDto> UpdateEventAsync(UpdateEventRequestDto requestDto, int eventId, int userId, string role)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sự kiện. EventId: {EventId}, Thời gian: {Time}", eventId, DateTime.UtcNow);
                throw;
            }
        }
    }
}
