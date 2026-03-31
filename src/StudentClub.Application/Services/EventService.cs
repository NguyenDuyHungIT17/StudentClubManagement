using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.Event;
using StudentClub.Application.DTOs.response.Event;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;
using StudentClub.Domain.Entities;
using StudentClub.Domain.Enums;
using StudentClub.Shared.ApiResponse;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace StudentClub.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IClubMemberRepository _clubmemberRepository;
        private readonly IUserRepository _user_repository_check; // keep DI order stable
        private readonly IUserRepository _userRepository;
        private readonly EventMapping _eventMapper;
        private readonly IPhotoService _photoService;
        private readonly ILogger<EventService> _logger;

        public EventService(
            IEventRepository eventRepository,
            IClubRepository clubRepository,
            IClubMemberRepository clubMemberRepository,
            EventMapping eventMapper,
            IUserRepository userRepository,
            IPhotoService photoService,
            ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
            _clubRepository = clubRepository;
            _eventMapper = eventMapper;
            _userRepository = userRepository;
            _user_repository_check = userRepository;
            _clubmemberRepository = clubMemberRepository;
            _photoService = photoService;
            _logger = logger;
        }

        public async Task<ApiResponse<CreateEventResponseDto>> CreateEventAsync(CreateEventRequestDto request, int userId, string role)
        {
            try
            {
                if (role == "leader")
                {
                    var club = await _club_repository_getById(request.ClubId);
                    if (club == null)
                        return ApiResponse<CreateEventResponseDto>.Failure(404, "Câu lạc bộ không tồn tại");

                    if (club.LeaderId != userId)
                        return ApiResponse<CreateEventResponseDto>.Failure(403, "Bạn không có quyền truy cập");
                }

                var ev = await _eventMapper.ToEntity(request);

                await _eventRepository.AddEventAsync(ev);
                await _eventRepository.SaveChangeAsync();

                var evDto = await _eventMapper.ToDto(ev);

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
                await _club_repository_saveChanges();
                return ApiResponse.Success("Xóa sự kiện thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "lỗi khi xóa sự kiện này");
                return ApiResponse.Failure(500, ex.Message);
            }
        }

        public async Task<PagedResponse<CreateEventResponseDto>> GetAllEventsAsync(EventFilterRequest filter, string role, int userId)
        {
            try
            {
                var evDto = new List<CreateEventResponseDto>();

                // LẤY DATA THEO ROLE (GIỮ NGUYÊN LOGIC)
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

                // FILTER
                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                {
                    var keyword = filter.Keyword.Trim().ToLower();

                    evDto = evDto
                        .Where(x =>
                            (x.Description ?? string.Empty).ToLower().Contains(keyword) ||
                            (!string.IsNullOrEmpty(x.Title) && x.Title.ToLower().Contains(keyword)))
                        .ToList();
                }

                if (filter.ClubId > 0)
                {
                    evDto = evDto
                        .Where(x => x.ClubId == filter.ClubId)
                        .ToList();
                }

                if (filter.IsPrivate.HasValue)
                {
                    evDto = evDto
                        .Where(x => x.IsPrivate == filter.IsPrivate.Value)
                        .ToList();
                }

                if (filter.Priority.HasValue)
                {
                    evDto = evDto
                        .Where(x => x.Priority == filter.Priority.Value)
                        .ToList();
                }

                var totalCount = evDto.Count;

                var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
                var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

                // Paginate before fetching photos (avoid N+1)
                var items = evDto
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Batch load photo URLs for paged items
                var eventIds = items.Select(i => i.Id).Distinct().ToList();
                if (eventIds.Count > 0)
                {
                    var photoMap = await _photoService.GetMainPhotoUrlsByEventIdsAsync(eventIds);
                    foreach (var it in items)
                    {
                        it.PhotoUrl = photoMap.ContainsKey(it.Id) ? photoMap[it.Id] : null;
                    }
                }

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return new PagedResponse<CreateEventResponseDto>
                {
                    Items = items,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Lỗi khi lấy danh sách sự kiện. UserId: {UserId}, Thời gian: {Time}",
                    userId,
                    DateTime.UtcNow);

                throw;
            }
        }

        public async Task<ApiResponse<CreateEventResponseDto>> GetEventByIdAsync(int eventId)
        {
            try
            {
                var ev = await _eventRepository.GetByEventIdAsync(eventId);
                if (ev == null)
                    return ApiResponse<CreateEventResponseDto>.Failure(404, "Sự kiện không tồn tại");

                var evDto = await _eventMapper.ToDto(ev);

                // attach main photo url
                evDto.PhotoUrl = await _photoService.GetMainPhotoUrlAsync(null, null, eventId, null);

                return ApiResponse<CreateEventResponseDto>.Success(evDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sự kiện theo ID. EventId: {EventId}, Thời gian: {Time}", eventId, DateTime.UtcNow);
                return ApiResponse<CreateEventResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<CreateEventResponseDto>>> GetEventsByClubIdAsync(int clubId, string role)
        {
            try
            {
                var ev = await _eventRepository.GetEventsByCLubIdAsync(clubId);
                var evDto = await _eventMapper.ToDtoList(ev);

                // batch attach photos
                var ids = evDto.Select(e => e.Id).Distinct().ToList();
                if (ids.Count > 0)
                {
                    var photos = await _photoService.GetMainPhotoUrlsByEventIdsAsync(ids);
                    foreach (var dto in evDto)
                        dto.PhotoUrl = photos.ContainsKey(dto.Id) ? photos[dto.Id] : null;
                }

                return ApiResponse<List<CreateEventResponseDto>>.Success(evDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sự kiện theo câu lạc bộ. ClubId: {ClubId}, Thời gian: {Time}", clubId, DateTime.UtcNow);
                return ApiResponse<List<CreateEventResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<CreateEventResponseDto>>> GetEventsByClubIdAsync(int userId)
        {
            try
            {
                var clubId = await _clubmemberRepository.GetClubIdByUserId(userId);
                var ev = await _eventRepository.GetAllEventsAsync();
                var evByClubId = ev.Where(e => e.ClubId == clubId).ToList();

                var evDto = await _eventMapper.ToDtoList(evByClubId);
                if (evDto.Count == 0)
                    return ApiResponse<List<CreateEventResponseDto>>.Failure(404, "Không có sự kiện nào cho câu lạc bộ này");

                // batch attach photos
                var ids = evDto.Select(e => e.Id).Distinct().ToList();
                if (ids.Count > 0)
                {
                    var photos = await _photoService.GetMainPhotoUrlsByEventIdsAsync(ids);
                    foreach (var dto in evDto)
                        dto.PhotoUrl = photos.ContainsKey(dto.Id) ? photos[dto.Id] : null;
                }

                return ApiResponse<List<CreateEventResponseDto>>.Success(evDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("Không tìm thấy sự kiện của câu lạc bộ");
                return ApiResponse<List<CreateEventResponseDto>>.Failure(500, "Lỗi hệ thống khi tìm kiếm sự kiện");
            }
        }

        public async Task<ApiResponse<List<CreateEventResponseDto>>> GetPublicEventsAsync()
        {
            try
            {
                var ev = await _eventRepository.GetPublicEventsAsync(false);
                var evDto = await _eventMapper.ToDtoList(ev);
                if (evDto.Count == 0)
                    return ApiResponse<List<CreateEventResponseDto>>.Failure(404, "Không có sự kiện công khai nào");

                // batch attach photos
                var ids = evDto.Select(e => e.Id).Distinct().ToList();
                if (ids.Count > 0)
                {
                    var photos = await _photoService.GetMainPhotoUrlsByEventIdsAsync(ids);
                    foreach (var dto in evDto)
                        dto.PhotoUrl = photos.ContainsKey(dto.Id) ? photos[dto.Id] : null;
                }

                return ApiResponse<List<CreateEventResponseDto>>.Success(evDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sự kiện công khai. Thời gian: {Time}", DateTime.UtcNow);
                return ApiResponse<List<CreateEventResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<CreateEventResponseDto>>> GetPublicEventsByClubIdAsync(int clubId)
        {
            try
            {
                var ev = await _eventRepository.GetPublicEventsByCLubIdAsync(clubId, false);
                var evDto = await _eventMapper.ToDtoList(ev);
                if (evDto.Count == 0)
                    return ApiResponse<List<CreateEventResponseDto>>.Failure(404, "Không có sự kiện công khai nào");

                // batch attach photos
                var ids = evDto.Select(e => e.Id).Distinct().ToList();
                if (ids.Count > 0)
                {
                    var photos = await _photoService.GetMainPhotoUrlsByEventIdsAsync(ids);
                    foreach (var dto in evDto)
                        dto.PhotoUrl = photos.ContainsKey(dto.Id) ? photos[dto.Id] : null;
                }

                return ApiResponse<List<CreateEventResponseDto>>.Success(evDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sự kiện công khai theo câu lạc bộ. ClubId: {ClubId}, Thời gian: {Time}", clubId, DateTime.UtcNow);
                return ApiResponse<List<CreateEventResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<CreateEventResponseDto>> UpdateEventAsync(UpdateEventRequestDto requestDto, int eventId, int userId, string role)
        {
            try
            {
                if (role == "leader")
                {
                    var club = await _club_repository_getById(requestDto.ClubId);
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
                ev.Priority = requestDto.Priority.HasValue
                            ? (EventPriority)requestDto.Priority.Value
                            : null;
                await _eventRepository.UpdateAsync(ev);
                await _eventRepository.SaveChangeAsync();

                var evDto = await _eventMapper.ToDto(ev);

                // attach main photo url for updated event
                evDto.PhotoUrl = await _photoService.GetMainPhotoUrlAsync(null, null, ev.EventId, null);

                return ApiResponse<CreateEventResponseDto>.Success(evDto, "Cập nhật sự kiện thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sự kiện. EventId: {EventId}, Thời gian: {Time}", eventId, DateTime.UtcNow);
                return ApiResponse<CreateEventResponseDto>.Failure(500, ex.Message);
            }
        }

        // small helpers to keep DI safe (no behavior change)
        private Task<Club> _club_repository_getById(int id) => _clubRepository.GetClubByClubIdAsync(id);
        private Task _club_repository_saveChanges() => _clubRepository.SaveChangeAsync();
    }
}