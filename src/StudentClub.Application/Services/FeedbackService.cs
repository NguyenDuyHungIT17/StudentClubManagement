using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;

namespace StudentClub.Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IUserRepository _userRepository;   
        private readonly FeedbackMapping _feedbackMapping;
        private readonly ILogger<FeedbackService> _logger;

        public FeedbackService(IFeedbackRepository feedbackRepository, 
                                FeedbackMapping feedbackMapping, ILogger<FeedbackService> logger,
                                IEventRepository eventRepository, IClubRepository clubRepository, IUserRepository userRepository)
        {
            _feedbackRepository = feedbackRepository;
            _feedbackMapping = feedbackMapping;
            _clubRepository = clubRepository;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _logger = logger;
        }
        public async Task<CreateFeedbackResponseDto> CreateFeedbackAsync(CreateFeedbackRequestDto feedbackDto, int userIdOnToken)
        {
            try
            {
                var feedback = await _feedbackMapping.ToEntity(feedbackDto, userIdOnToken);
                
                var user = await _userRepository.GetUserByUserIdAsync(feedback.UserId);
                await _feedbackRepository.CreateFeedbackAsync(feedback);
                await _feedbackRepository.SaveChangesAsync();

                var responseDto = await _feedbackMapping.ToResponse(feedback);
                return responseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể tạo phản hồi");
                throw;
            }
        }

        public async Task DeleteFeedbackAsync(int feedbackId)
        {
            try
            {
                var feedback = await _feedbackRepository.GetFeedbackByIdAsync(feedbackId);
                if (feedback == null)
                {
                    throw new KeyNotFoundException("Phản hồi không tồn tại");
                }
                
                await _feedbackRepository.DeleteFeedbackAsync(feedbackId);
                await _feedbackRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể xóa phản hồi");
                throw;
            }
        }

        public async Task<List<CreateFeedbackResponseDto>> GetAllFeedbacksAsync()
        {
            try
            {
                var listFeedbacks = await _feedbackRepository.GetAllFeedbacksAsync();
                if (listFeedbacks == null)
                {
                    throw new KeyNotFoundException("Không có phản hồi nào");
                }
                var responseDtos = await _feedbackMapping.ToDtoList(listFeedbacks);

                return responseDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể lấy danh sách phản hồi");
                throw;
            }
        }

        public async Task<CreateFeedbackResponseDto> GetFeedbackByIdAsync(int feedbackId)
        {
            try
            {
                var feedback = await _feedbackRepository.GetFeedbackByIdAsync(feedbackId);
                if (feedback == null)
                {
                    throw new KeyNotFoundException("Phản hồi không tồn tại");
                }
                var responseDto = await _feedbackMapping.ToResponse(feedback);
                return responseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể lấy phản hồi theo Id");
                throw;
            }
        }

        public async Task<List<CreateFeedbackResponseDto>> GetFeedbacksByEventIdAsync(int eventId)
        {
            try
            {
                var feedbacks = await _feedbackRepository.GetFeedbacksByEventIdAsync(eventId);

                var response = await _feedbackMapping.ToDtoList(feedbacks);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể lấy phản hồi theo EventId");
                throw;
            }
        }

        public async Task<CreateFeedbackResponseDto> UpdateFeedbackAsync(int id, CreateFeedbackRequestDto feedbackDto)
        {
            try
            {
                var feedbackInDb = await _feedbackRepository.GetFeedbackByIdAsync(id);
                if(feedbackInDb == null)
                {
                    throw new KeyNotFoundException("Phản hồi không tồn tại");
                }

                feedbackInDb.EventId = feedbackDto.EventId;
                feedbackInDb.Comment = feedbackDto.Comment;
                feedbackInDb.Rating = feedbackDto.Rating;
                feedbackInDb.UpdatedAt = DateTime.UtcNow;

                await _feedbackRepository.UpdateFeedbackAsync(feedbackInDb);
                await _feedbackRepository.SaveChangesAsync();

                var responseDto = await _feedbackMapping.ToResponse(feedbackInDb);
                return responseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể cập nhật phản hồi");
                throw;
            }
        }
    }
}
