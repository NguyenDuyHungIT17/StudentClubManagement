using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request.Event;
using StudentClub.Application.DTOs.response.Event;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;
using StudentClub.Domain.Entities;
using StudentClub.Shared.ApiResponse; // Thêm namespace này

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

        public async Task<ApiResponse<CreateEventResponseDto>> CreateEventAsync(CreateEventRequestDto request, int userId, string role)
        {
            try
            {
                if (role == "leader")
                {
                    var club = await _clubRepository.GetClubByClubIdAsync(request.ClubId);
                    if (club == null)
                        return ApiResponse<CreateEventResponseDto>.Failure(404, "Câu lạc bộ không tồn tại");

                    if (club.LeaderId != userId)
                        return ApiResponse<CreateEventResponseDto>.Failure(403, "Bạn không có quyền truy cập");
                }

                var ev = new Event
                {
                    EventDate = request.EventDate,
                    ClubId = request.ClubId,
                    Description = request.Description,
                    Title = request.Title,
                    Priority = request.Priority,
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
                    Priority = ev.Priority
                };

                return ApiResponse<CreateEventResponseDto>.Success(evDto, "Tạo sự kiện thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo sự kiện. Tên sự kiện: {EventName}, Thời gian: {Time}", request.Title, DateTime.UtcNow);
                return ApiResponse<CreateEventResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteEvent(int id)
        {
            try
            {
                await _eventRepository.DeleteEvent(id);
                await _clubRepository.SaveChangeAsync();
                return ApiResponse.Success("Xóa sự kiện thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "lỗi khi xóa sự kiện này");
                return ApiResponse.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<GetAllEventsResponseDto>>> GetAllEventsAsync(string role, int userId)
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
                    evDto = await _eventMapper.ToDtoList(ev);
                }
                return ApiResponse<List<GetAllEventsResponseDto>>.Success(evDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tất cả sự kiện. UserId: {UserId}, Thời gian: {Time}", userId, DateTime.UtcNow);
                return ApiResponse<List<GetAllEventsResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<GetAllEventsResponseDto>> GetEventByIdAsync(int eventId)
        {
            try
            {
                var ev = await _eventRepository.GetByEventIdAsync(eventId);
                if (ev == null)
                    return ApiResponse<GetAllEventsResponseDto>.Failure(404, "Sự kiện không tồn tại");

                var evDto = await _eventMapper.ToDto(ev);
                return ApiResponse<GetAllEventsResponseDto>.Success(evDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sự kiện theo ID. EventId: {EventId}, Thời gian: {Time}", eventId, DateTime.UtcNow);
                return ApiResponse<GetAllEventsResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<GetAllEventsResponseDto>>> GetEventsByClubIdAsync(int clubId, string role)
        {
            try
            {
                var ev = await _eventRepository.GetEventsByCLubIdAsync(clubId);
                var evDto = await _eventMapper.ToDtoList(ev);
                return ApiResponse<List<GetAllEventsResponseDto>>.Success(evDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sự kiện theo câu lạc bộ. ClubId: {ClubId}, Thời gian: {Time}", clubId, DateTime.UtcNow);
                return ApiResponse<List<GetAllEventsResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<GetAllEventsResponseDto>>> GetEventsByClubIdAsync(int userId)
        {
            try
            {
                var clubId = await _clubmemberRepository.GetClubIdByUserId(userId);
                var ev = await _eventRepository.GetAllEventsAsync();
                var evByClubId = ev.Where(e => e.ClubId == clubId).ToList();

                var evDto = await _eventMapper.ToDtoList(evByClubId);
                if (evDto.Count == 0)
                    return ApiResponse<List<GetAllEventsResponseDto>>.Failure(404, "Không có sự kiện nào cho câu lạc bộ này");

                return ApiResponse<List<GetAllEventsResponseDto>>.Success(evDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("Không tìm thấy sự kiện của câu lạc bộ");
                return ApiResponse<List<GetAllEventsResponseDto>>.Failure(500, "Lỗi hệ thống khi tìm kiếm sự kiện");
            }
        }

        public async Task<ApiResponse<List<GetAllEventsResponseDto>>> GetPublicEventsAsync()
        {
            try
            {
                var ev = await _eventRepository.GetPublicEventsAsync(false);
                var evDto = await _eventMapper.ToDtoList(ev);
                if (evDto.Count == 0)
                    return ApiResponse<List<GetAllEventsResponseDto>>.Failure(404, "Không có sự kiện công khai nào");

                return ApiResponse<List<GetAllEventsResponseDto>>.Success(evDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sự kiện công khai. Thời gian: {Time}", DateTime.UtcNow);
                return ApiResponse<List<GetAllEventsResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<GetAllEventsResponseDto>>> GetPublicEventsByClubIdAsync(int clubId)
        {
            try
            {
                var ev = await _eventRepository.GetPublicEventsByCLubIdAsync(clubId, false);
                var evDto = await _eventMapper.ToDtoList(ev);
                if (evDto.Count == 0)
                    return ApiResponse<List<GetAllEventsResponseDto>>.Failure(404, "Không có sự kiện công khai nào");

                return ApiResponse<List<GetAllEventsResponseDto>>.Success(evDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sự kiện công khai theo câu lạc bộ. ClubId: {ClubId}, Thời gian: {Time}", clubId, DateTime.UtcNow);
                return ApiResponse<List<GetAllEventsResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<CreateEventResponseDto>> UpdateEventAsync(UpdateEventRequestDto requestDto, int eventId, int userId, string role)
        {
            try
            {
                if (role == "leader")
                {
                    var club = await _clubRepository.GetClubByClubIdAsync(requestDto.ClubId);
                    if (club == null)
                        return ApiResponse<CreateEventResponseDto>.Failure(404, "Câu lạc bộ không tồn tại");

                    if (club.LeaderId != userId)
                        return ApiResponse<CreateEventResponseDto>.Failure(403, "Bạn không có quyền truy cập");
                }

                var ev = await _eventRepository.GetByEventIdAsync(eventId);
                if (ev == null)
                    return ApiResponse<CreateEventResponseDto>.Failure(404, "Sự kiện không tồn tại");

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

                return ApiResponse<CreateEventResponseDto>.Success(evDto, "Cập nhật sự kiện thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sự kiện. EventId: {EventId}, Thời gian: {Time}", eventId, DateTime.UtcNow);
                return ApiResponse<CreateEventResponseDto>.Failure(500, ex.Message);
            }
        }
    }
}