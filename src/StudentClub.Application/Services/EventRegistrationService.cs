using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;

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
        public async Task<CreateEventRegistrationResponseDto> CreateEventRegistrationAsync(CreateEventRegistrationRequestDto request, int userId)
        {
            try
            {
                if (userId != 0)
                {
                    var user = await _userRepository.GetUserByUserIdAsync(userId);
                    if (user == null)
                    {
                        throw new KeyNotFoundException("Người dùng không tồn tại");
                    }

                    var entity = await _eventMapping.MapToEntity(request);
                    await _eventRegistrationRepository.AddEventRegistrationAsync(entity);
                    await _eventRegistrationRepository.SaveChangeAsynce();
                    
                    var responseDto = await _eventMapping.MapToCreateEventRegistrationResponseDto(entity);

                    return responseDto;
                }
                else
                {
                    request.UserId = 22;

                    var entity = await _eventMapping.MapToEntity(request);
                    await _eventRegistrationRepository.AddEventRegistrationAsync(entity);
                    await _eventRegistrationRepository.SaveChangeAsynce();

                    var responseDto = await _eventMapping.MapToCreateEventRegistrationResponseDto(entity);

                    return responseDto;
                }
               
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Lỗi khi thêm người tham gia sự kiện Thời gian: {Time}",  DateTime.UtcNow);
                throw;
            }
        }

        public async Task DeleteEventRegistration(int eventRegistrationId, string role, int userId)
        {
            try
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
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa thành viên - sự kiện, Thời gian: {Time}", DateTime.UtcNow);
                throw;
            }
          
        }

        public async Task<List<CreateEventRegistrationResponseDto>> GetAllEventRegistrationsByEventId(int eventId)
        {
            try
            {
                var ers = await _eventRegistrationRepository.GetEventRegistrationByEventIdAsync(eventId);
                if (ers == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy thông tin");
                }
                var ersDto = new List<CreateEventRegistrationResponseDto>();
                ersDto = await _eventMapping.MapToDtoList(ers);
                return ersDto;
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thành viên - sự kiện, Thời gian: {Time}", DateTime.UtcNow);
                throw;
            }
        }
    }
}
