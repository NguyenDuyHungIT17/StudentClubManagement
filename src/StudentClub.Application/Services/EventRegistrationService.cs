using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request.EventRegistration;
using StudentClub.Application.DTOs.response.EventRegistration;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;
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
                if (userId != 0)
                {
                    var user = await _userRepository.GetUserByUserIdAsync(userId);
                    if (user == null)
                    {
                        return ApiResponse<CreateEventRegistrationResponseDto>.Failure(404, "Người dùng không tồn tại");
                    }

                    var entity = await _eventMapping.MapToEntity(request);
                    await _eventRegistrationRepository.AddEventRegistrationAsync(entity);
                    await _eventRegistrationRepository.SaveChangeAsynce();

                    var responseDto = await _eventMapping.MapToCreateEventRegistrationResponseDto(entity);
                    return ApiResponse<CreateEventRegistrationResponseDto>.Success(responseDto, "Đăng ký sự kiện thành công");
                }
                else
                {
                    request.UserId = 22;

                    var entity = await _eventMapping.MapToEntity(request);
                    await _eventRegistrationRepository.AddEventRegistrationAsync(entity);
                    await _eventRegistrationRepository.SaveChangeAsynce();

                    var responseDto = await _eventMapping.MapToCreateEventRegistrationResponseDto(entity);
                    return ApiResponse<CreateEventRegistrationResponseDto>.Success(responseDto, "Đăng ký sự kiện thành công (Mặc định)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm người tham gia sự kiện Thời gian: {Time}", DateTime.UtcNow);
                return ApiResponse<CreateEventRegistrationResponseDto>.Failure(500, ex.Message);
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

                if (role == "leader")
                {
                    var club = await _clubRepository.GetClubByClubIdAsync(ev.ClubId);
                    if (club == null)
                        return ApiResponse.Failure(404, "Câu lạc bộ không tồn tại");

                    if (club.LeaderId != userId)
                        return ApiResponse.Failure(403, "Bạn không có quyền xóa sự kiện này");

                    await _eventRegistrationRepository.DeleteEventRegistrationAsync(eventRegistrationId);
                }
                else if (role == "admin")
                {
                    await _eventRegistrationRepository.DeleteEventRegistrationAsync(eventRegistrationId);
                }
                else
                {
                    return ApiResponse.Failure(403, "Vai trò của bạn không được phép thực hiện hành động này");
                }

                await _eventRegistrationRepository.SaveChangeAsynce(); // Đảm bảo có lưu thay đổi nếu repository chưa lưu
                return ApiResponse.Success("Xóa đăng ký sự kiện thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa thành viên - sự kiện, Thời gian: {Time}", DateTime.UtcNow);
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