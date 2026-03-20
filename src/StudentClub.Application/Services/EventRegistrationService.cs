using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.EventRegistration;
using StudentClub.Application.DTOs.response.EventRegistration;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;
using StudentClub.Domain.Enums;
using StudentClub.Shared.ApiResponse; // Thêm namespace này

namespace StudentClub.Application.Services
{
    public class EventRegistrationService : IEventRegistrationService
    {
        private readonly IEventRegistrationRepository _eventRegistrationRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly EventRegistrationMapping _eventMapping;
        private readonly ILogger<EventRegistrationService> _logger;

        public EventRegistrationService(IEventRegistrationRepository eventRegistrationRepository, EventRegistrationMapping eventMapping, IClubRepository clubRepository, IEventRepository eventRepository, ILogger<EventRegistrationService> logger, IUserRepository userRepository)
        {
            _eventRegistrationRepository = eventRegistrationRepository;
            _eventMapping = eventMapping;
            _clubRepository = clubRepository;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _logger = logger;
        }


        public async Task<ApiResponse> DeleteEventRegistration(int eventRegistrationId, string role, int userId)
        {
            try
            {
                var er = await _eventRegistrationRepository.GetEventRegistrationByIdAsync(eventRegistrationId);
                if (er == null)
                {
                    return ApiResponse.Failure(404, "Không tìm thấy thông tin đăng ký");
                }

                var ev = await _eventRepository.GetEventByIdAsync(er.EventId);
                if (ev == null)
                {
                    return ApiResponse.Failure(404, "Sự kiện không tồn tại");
                }

                bool hasPermission = false;


                if (role == RoleConstants.Admin)
                {
                    hasPermission = true;
                }

                else if (role == RoleConstants.Leader)
                {
                    var club = await _clubRepository.GetClubByClubIdAsync(ev.ClubId);
                    if (club == null)
                    {
                        return ApiResponse.Failure(404, "Câu lạc bộ không tồn tại");
                    }

                    if (club.LeaderId == userId)
                    {
                        hasPermission = true;
                    }
                }

                else if (role == RoleConstants.Member)
                {
                    if (er.UserId == userId)
                    {
                        hasPermission = true;
                    }
                }

                if (!hasPermission)
                {
                    return ApiResponse.Failure(403, "Bạn không có quyền xóa đăng ký này");
                }

                await _eventRegistrationRepository.DeleteEventRegistrationAsync(eventRegistrationId);
                await _eventRegistrationRepository.SaveChangeAsynce();

                return ApiResponse.Success("Xóa đăng ký sự kiện thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa đăng ký sự kiện, Thời gian: {Time}", DateTime.UtcNow);
                return ApiResponse.Failure(500, ex.Message);
            }
        }

        public async Task<PagedResponse<CreateEventRegistrationResponseDto>> GetAllEventRegistrationsByEventId(int eventId, EventRegistrationFilter filter)
        {
            try
            {
                var ers = await _eventRegistrationRepository.GetEventRegistrationByEventIdAsync(eventId);

                if (ers == null || !ers.Any())
                {
                    return new PagedResponse<CreateEventRegistrationResponseDto>
                    {
                        Items = new List<CreateEventRegistrationResponseDto>(),
                        PageNumber = 1,
                        PageSize = 10,
                        TotalPages = 0,
                        TotalCount = 0
                    };
                }
                var ersDto = ers.Select(x => new CreateEventRegistrationResponseDto
                {
                    Id = x.RegistrationId,
                    EventId = x.EventId,
                    UserId = x.UserId,
                    GuestEmail = x.GuestEmail,
                    GuestName = x.GuestName,
                    CheckedIn = x.CheckedIn,
                    CheckName = x.CheckName,
                    IsCare = x.IsCare,
                    RegisteredAt = x.RegisteredAt,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                }).ToList();

                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    var keyword = filter.KeyWord.Trim().ToLower();

                    ersDto = ersDto
                        .Where(x =>
                            (!string.IsNullOrEmpty(x.GuestName) && x.GuestName.ToLower().Contains(keyword)) ||
                            (!string.IsNullOrEmpty(x.GuestEmail) && x.GuestEmail.ToLower().Contains(keyword))
                        )
                        .ToList();
                }

                var totalCount = ersDto.Count;

                var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
                var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

                var items = ersDto
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return new PagedResponse<CreateEventRegistrationResponseDto>
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
                _logger.LogError(ex, "Lỗi khi lấy danh sách đăng ký sự kiện. Thời gian: {Time}", DateTime.UtcNow);
                throw;
            }
        }

        public async Task<ApiResponse<CreateEventRegistrationResponseDto>> GetById(int id)
        {
            try
            {
                var er = await _eventRegistrationRepository.GetEventRegistrationByIdAsync(id);

                if (er == null)
                {
                    return ApiResponse<CreateEventRegistrationResponseDto>
                        .Failure(404, "Không tìm thấy thông tin đăng ký");
                }

                var dto = await _eventMapping.MapToCreateEventRegistrationResponseDto(er);

                return ApiResponse<CreateEventRegistrationResponseDto>
                    .Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết đăng ký sự kiện, Thời gian: {Time}", DateTime.UtcNow);
                return ApiResponse<CreateEventRegistrationResponseDto>
                    .Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<CreateEventRegistrationResponseDto>> CreateEventRegistrationAsync(CreateEventRegistrationRequestDto request, int currentUserId)
        {
            try
            {
                // Ưu tiên lấy UserId từ Frontend truyền lên. 
                // Nếu Frontend truyền null và không nhập email khách -> lấy ID của người đang đăng nhập (trường hợp tự đăng ký)
                int targetUserId = 0;

                if (request.UserId.HasValue && request.UserId.Value > 0)
                {
                    targetUserId = request.UserId.Value;
                }
                else if (!request.UserId.HasValue && string.IsNullOrWhiteSpace(request.GuestEmail))
                {
                    targetUserId = currentUserId;
                }

                if (targetUserId > 0)
                {
                    var user = await _userRepository.GetUserByUserIdAsync(targetUserId);
                    if (user == null)
                    {
                        return ApiResponse<CreateEventRegistrationResponseDto>
                            .Failure(404, "Người dùng không tồn tại");
                    }

                    request.UserId = targetUserId;
                    request.GuestEmail = null;
                    request.GuestName = null;
                }
                else
                {
                    request.UserId = null; // Là khách vãng lai
                    if (string.IsNullOrWhiteSpace(request.GuestEmail) ||
                        string.IsNullOrWhiteSpace(request.GuestName))
                    {
                        return ApiResponse<CreateEventRegistrationResponseDto>
                            .Failure(400, "Khách phải nhập Email và Tên");
                    }
                }

                var entity = await _eventMapping.MapToEntity(request);

                await _eventRegistrationRepository.AddEventRegistrationAsync(entity);
                await _eventRegistrationRepository.SaveChangeAsynce();

                var responseDto = await _eventMapping.MapToCreateEventRegistrationResponseDto(entity);

                return ApiResponse<CreateEventRegistrationResponseDto>
                    .Success(responseDto, "Đăng ký sự kiện thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm người tham gia sự kiện Thời gian: {Time}", DateTime.UtcNow);

                return ApiResponse<CreateEventRegistrationResponseDto>
                    .Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<CreateEventRegistrationResponseDto>> Update(int id, CreateEventRegistrationRequestDto request, string role, int currentUserId)
        {
            try
            {
                var er = await _eventRegistrationRepository.GetEventRegistrationByIdAsync(id);
                if (er == null)
                {
                    return ApiResponse<CreateEventRegistrationResponseDto>
                        .Failure(404, "Không tìm thấy thông tin đăng ký");
                }

                var ev = await _eventRepository.GetEventByIdAsync(er.EventId);
                if (ev == null)
                {
                    return ApiResponse<CreateEventRegistrationResponseDto>
                        .Failure(404, "Sự kiện không tồn tại");
                }

                // Check quyền
                bool hasPermission = false;
                if (role == RoleConstants.Admin)
                {
                    hasPermission = true;
                }
                else if (role == RoleConstants.Leader)
                {
                    var club = await _clubRepository.GetClubByClubIdAsync(ev.ClubId);
                    if (club == null)
                    {
                        return ApiResponse<CreateEventRegistrationResponseDto>.Failure(404, "Câu lạc bộ không tồn tại");
                    }
                    if (club.LeaderId == currentUserId)
                    {
                        hasPermission = true;
                    }
                }
                else if (role == RoleConstants.Member)
                {
                    if (er.UserId == currentUserId)
                    {
                        hasPermission = true;
                    }
                }

                if (!hasPermission)
                {
                    return ApiResponse<CreateEventRegistrationResponseDto>
                        .Failure(403, "Bạn không có quyền chỉnh sửa đăng ký này");
                }

                // Cập nhật người tham gia (Dùng UserId do Admin truyền từ Frontend)
                int targetUserId = (request.UserId.HasValue && request.UserId.Value > 0) ? request.UserId.Value : 0;

                if (targetUserId > 0)
                {
                    request.UserId = targetUserId;
                    request.GuestEmail = null;
                    request.GuestName = null;
                }
                else
                {
                    request.UserId = null;
                    if (string.IsNullOrWhiteSpace(request.GuestEmail) ||
                        string.IsNullOrWhiteSpace(request.GuestName))
                    {
                        return ApiResponse<CreateEventRegistrationResponseDto>
                            .Failure(400, "Khách phải nhập Email và Tên");
                    }
                }

                er.UserId = request.UserId;
                er.GuestEmail = request.GuestEmail;
                er.GuestName = request.GuestName;
                er.IsCare = request.IsCare;
                er.CheckedIn = request.CheckedIn;
                er.CheckName = request.CheckName;
                er.UpdatedAt = DateTime.UtcNow;

                await _eventRegistrationRepository.UpdateEventRegistrationAsync(er);
                await _eventRegistrationRepository.SaveChangeAsynce();

                var responseDto = await _eventMapping.MapToCreateEventRegistrationResponseDto(er);

                return ApiResponse<CreateEventRegistrationResponseDto>
                    .Success(responseDto, "Cập nhật đăng ký thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật đăng ký sự kiện, Thời gian: {Time}", DateTime.UtcNow);
                return ApiResponse<CreateEventRegistrationResponseDto>
                    .Failure(500, ex.Message);
            }

        }
    }
}