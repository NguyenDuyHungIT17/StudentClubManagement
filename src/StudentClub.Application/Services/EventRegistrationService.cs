using Microsoft.Extensions.Logging;
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

        public async Task<ApiResponse<CreateEventRegistrationResponseDto>> CreateEventRegistrationAsync(CreateEventRegistrationRequestDto request, int userId)
        {
            try
            {
                if (userId > 0)
                {
                    var user = await _userRepository.GetUserByUserIdAsync(userId);
                    if (user == null)
                    {
                        return ApiResponse<CreateEventRegistrationResponseDto>
                            .Failure(404, "Người dùng không tồn tại");
                    }

                    request.UserId = userId; 

                    request.GuestEmail = null;
                    request.GuestName = null;
                }
                else
                {
                    request.UserId = 0;
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

        public async Task<ApiResponse<List<CreateEventRegistrationResponseDto>>> GetAllEventRegistrationsByEventId(int eventId)
        {
            try
            {
                var ers = await _eventRegistrationRepository.GetEventRegistrationByEventIdAsync(eventId);
                if (ers == null || !ers.Any())
                {
                    return ApiResponse<List<CreateEventRegistrationResponseDto>>.Failure(404, "Không tìm thấy thông tin");
                }

                var ersDto = await _eventMapping.MapToDtoList(ers);
                return ApiResponse<List<CreateEventRegistrationResponseDto>>.Success(ersDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thành viên - sự kiện, Thời gian: {Time}", DateTime.UtcNow);
                return ApiResponse<List<CreateEventRegistrationResponseDto>>.Failure(500, ex.Message);
            }
        }
    }
}