namespace StudentClub.Application.DTOs.Response.Interview
{
    public class InterviewResponseDto
    {
        public int InterviewId { get; set; }

        public int ClubId { get; set; }

        public string ApplicantName { get; set; } = null!;

        public string? ApplicantEmail { get; set; }

        public string? ApplicantPhone { get; set; }

        public int ApplicationType { get; set; }

        public DateTime? InterviewDate { get; set; }

        public DateTime? CheckInTime { get; set; }

        public int Status { get; set; }

        public int Result { get; set; }

        public string? Evaluation { get; set; }

        public int? EvaluatorId { get; set; }

        public string? EvaluatorName { get; set; }

        public string? CVUrl { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}