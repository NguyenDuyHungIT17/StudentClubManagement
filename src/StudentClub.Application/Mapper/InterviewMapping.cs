using StudentClub.Application.DTOs.Request.Interview;
using StudentClub.Application.DTOs.Response.Interview;
using StudentClub.Domain.Entities;
using StudentClub.Domain.Enums;

namespace StudentClub.Application.Mapper
{
    public class InterviewMapping
    {
        // Map create request to entity
        public Interview ToEntity(
            CreateInterviewRequestDto dto,
            InterviewStatus status,
            ApplicationType type
            )
        {
            return new Interview
            {
                ClubId = dto.ClubId,
                ApplicantName = dto.ApplicantName,
                ApplicantEmail = dto.ApplicantEmail,
                ApplicantPhone = dto.ApplicantPhone,
                InterviewDate = dto.InterviewDate,
                ApplicationType = type,
                CVUrl = dto.CVUrl,
                Note = dto.Note,
                Status = status,
                Result = 0,
                CreatedAt = DateTime.UtcNow,
                CampaignId = dto.CampaignId
            };
        }

        // Map update request to entity
        public void UpdateEntity(Interview entity, UpdateInterviewRequestDto dto)
        {
            entity.ApplicantName = dto.ApplicantName;
            entity.ApplicantEmail = dto.ApplicantEmail;
            entity.ApplicantPhone = dto.ApplicantPhone;
            entity.InterviewDate = dto.InterviewDate;
            entity.CVUrl = dto.CVUrl;
            entity.Note = dto.Note;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        // Map start interview
        public void MapStart(Interview entity, StartInterviewRequestDto dto)
        {
            entity.EvaluatorId = dto.EvaluatorId;
            entity.EvaluatorName = dto.EvaluatorName;
            entity.Status = InterviewStatus.Interviewing;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        // Map finish interview
        public void MapFinish(Interview entity, FinishInterviewRequestDto dto)
        {
            entity.Result = (InterviewResult)dto.Result;
            entity.Evaluation = dto.Evaluation;
            entity.Note = dto.Note;
            entity.Status = InterviewStatus.Done;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        // Map check-in
        public void MapCheckIn(Interview entity)
        {
            entity.CheckInTime = DateTime.UtcNow;
            entity.Status = InterviewStatus.CheckedIn;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        // Map no-show
        public void MapNoShow(Interview entity, string? reason)
        {
            entity.Status = InterviewStatus.NoShow;
            entity.Note = reason;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        // Map cancel interview
        public void MapCancel(Interview entity)
        {
            entity.Status = InterviewStatus.Cancelled;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        // Map entity to response
        public InterviewResponseDto ToResponse(Interview entity)
        {
            return new InterviewResponseDto
            {
                InterviewId = entity.InterviewId,
                ClubId = entity.ClubId,
                ApplicantName = entity.ApplicantName,
                ApplicantEmail = entity.ApplicantEmail,
                ApplicantPhone = entity.ApplicantPhone,
                ApplicationType = (int)entity.ApplicationType,
                InterviewDate = entity.InterviewDate,
                CheckInTime = entity.CheckInTime,
                Status = (int)entity.Status,
                Result = (int)entity.Result,
                Evaluation = entity.Evaluation,
                EvaluatorId = entity.EvaluatorId,
                EvaluatorName = entity.EvaluatorName,
                CVUrl = entity.CVUrl,
                Note = entity.Note,
                CampaignId = entity.CampaignId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        // Map list entity to response list
        public List<InterviewResponseDto> ToListResponse(List<Interview> entities)
        {
            return entities.Select(x => ToResponse(x)).ToList();
        }
    }
}