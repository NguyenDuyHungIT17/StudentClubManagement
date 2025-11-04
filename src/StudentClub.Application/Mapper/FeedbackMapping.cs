using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;

namespace StudentClub.Application.Mapper
{
    public class FeedbackMapping
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IEventRepository _eventRepository;
        public FeedbackMapping(IFeedbackRepository feedbackRepository, IEventRepository eventRepository)
        {
            _feedbackRepository = feedbackRepository;
            _eventRepository = eventRepository;
        }

        public async virtual Task<CreateFeedbackResponseDto> ToResponse(Feedback feedback)
        {
            return new CreateFeedbackResponseDto
            {
                EventName = await _eventRepository.GetEventNameByIdAsync(feedback.EventId),
                Comment = feedback.Comment,
                Rating = feedback.Rating,
                CreatedAt = DateTime.Now
            };
        }
        public async virtual Task<List<CreateFeedbackResponseDto>> ToDtoList(List<Feedback> feedback)
        {
            var result = new List<CreateFeedbackResponseDto>();
            foreach (var item in feedback)
            {
                var dto = await ToResponse(item);
                result.Add(dto);
            }
            return result;
        }

        public async virtual Task<Feedback> ToEntity(CreateFeedbackRequestDto request, int userIdOnToken)
        {
            var userId = 17;
            if (userIdOnToken != 0)
            {
                userId = (int)userIdOnToken;
            }
            return new Feedback
            {
                    UserId = userId,
                    EventId = request.EventId,
                    Comment = request.Comment,
                    Rating = request.Rating
            };
        }
    }
}
