using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request.Feedback;
using StudentClub.Application.DTOs.response.Feedback;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;
using StudentClub.Shared.ApiResponse; // Thêm namespace này

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

        public async Task<ApiResponse<CreateFeedbackResponseDto>> CreateFeedbackAsync(CreateFeedbackRequestDto feedbackDto, int userIdOnToken)
        {
            try
            {
                var feedback = await _feedbackMapping.ToEntity(feedbackDto, userIdOnToken);

                var user = await _userRepository.GetUserByUserIdAsync(feedback.UserId);
                await _feedbackRepository.CreateFeedbackAsync(feedback);
                await _feedbackRepository.SaveChangesAsync();

                var responseDto = await _feedbackMapping.ToResponse(feedback);
                return ApiResponse<CreateFeedbackResponseDto>.Success(responseDto, "Tạo phản hồi thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể tạo phản hồi");
                return ApiResponse<CreateFeedbackResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteFeedbackAsync(int feedbackId)
        {
            try
            {
                var feedback = await _feedbackRepository.GetFeedbackByIdAsync(feedbackId);
                if (feedback == null)
                {
                    return ApiResponse.Failure(404, "Phản hồi không tồn tại");
                }

                await _feedbackRepository.DeleteFeedbackAsync(feedbackId);
                await _feedbackRepository.SaveChangesAsync();
                return ApiResponse.Success("Xóa phản hồi thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể xóa phản hồi");
                return ApiResponse.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<CreateFeedbackResponseDto>>> GetAllFeedbacksAsync()
        {
            try
            {
                var listFeedbacks = await _feedbackRepository.GetAllFeedbacksAsync();
                if (listFeedbacks == null || !listFeedbacks.Any())
                {
                    return ApiResponse<List<CreateFeedbackResponseDto>>.Failure(404, "Không có phản hồi nào");
                }
                var responseDtos = await _feedbackMapping.ToDtoList(listFeedbacks);

                return ApiResponse<List<CreateFeedbackResponseDto>>.Success(responseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể lấy danh sách phản hồi");
                return ApiResponse<List<CreateFeedbackResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<CreateFeedbackResponseDto>> GetFeedbackByIdAsync(int feedbackId)
        {
            try
            {
                var feedback = await _feedbackRepository.GetFeedbackByIdAsync(feedbackId);
                if (feedback == null)
                {
                    return ApiResponse<CreateFeedbackResponseDto>.Failure(404, "Phản hồi không tồn tại");
                }
                var responseDto = await _feedbackMapping.ToResponse(feedback);
                return ApiResponse<CreateFeedbackResponseDto>.Success(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể lấy phản hồi theo Id");
                return ApiResponse<CreateFeedbackResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<CreateFeedbackResponseDto>>> GetFeedbacksByEventIdAsync(int eventId)
        {
            try
            {
                var feedbacks = await _feedbackRepository.GetFeedbacksByEventIdAsync(eventId);
                var response = await _feedbackMapping.ToDtoList(feedbacks);

                return ApiResponse<List<CreateFeedbackResponseDto>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể lấy phản hồi theo EventId");
                return ApiResponse<List<CreateFeedbackResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<CreateFeedbackResponseDto>> UpdateFeedbackAsync(int id, CreateFeedbackRequestDto feedbackDto)
        {
            try
            {
                var feedbackInDb = await _feedbackRepository.GetFeedbackByIdAsync(id);
                if (feedbackInDb == null)
                {
                    return ApiResponse<CreateFeedbackResponseDto>.Failure(404, "Phản hồi không tồn tại");
                }

                feedbackInDb.EventId = feedbackDto.EventId;
                feedbackInDb.Comment = feedbackDto.Comment;
                feedbackInDb.Rating = feedbackDto.Rating;
                feedbackInDb.UpdatedAt = DateTime.UtcNow;

                await _feedbackRepository.UpdateFeedbackAsync(feedbackInDb);
                await _feedbackRepository.SaveChangesAsync();

                var responseDto = await _feedbackMapping.ToResponse(feedbackInDb);
                return ApiResponse<CreateFeedbackResponseDto>.Success(responseDto, "Cập nhật phản hồi thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể cập nhật phản hồi");
                return ApiResponse<CreateFeedbackResponseDto>.Failure(500, ex.Message);
            }
        }
    }
}