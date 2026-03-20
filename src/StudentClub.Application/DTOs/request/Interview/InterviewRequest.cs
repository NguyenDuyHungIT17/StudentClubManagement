namespace StudentClub.Application.DTOs.Request.Interview
{
    // khi ứng viên đăng kí
    public class CreateInterviewRequestDto
    {
        public int ClubId { get; set; }

        public string ApplicantName { get; set; } = null!;

        public string? ApplicantEmail { get; set; }

        public string? ApplicantPhone { get; set; }

        public DateTime? InterviewDate { get; set; }

        public string? CVUrl { get; set; }

        public string? Note { get; set; }
    }

    //bắt đầu phỏng vấn
    public class StartInterviewRequestDto
    {
        public int EvaluatorId { get; set; }

        public string? EvaluatorName { get; set; }
    }

    //kết thúc phỏng vấn
    public class FinishInterviewRequestDto
    {
        /// <summary>
        /// 1 = Pass, 2 = Fail
        /// </summary>
        public int Result { get; set; }

        public string? Evaluation { get; set; }

        public string? Note { get; set; }
    }

    //không đến phỏng vấn
    public class NoShowInterviewRequestDto
    {
        public string? Reason { get; set; }
    }

    // update lại ứng viên
    public class UpdateInterviewRequestDto
    {
        public string ApplicantName { get; set; } = null!;

        public string? ApplicantEmail { get; set; }

        public string? ApplicantPhone { get; set; }

        public DateTime? InterviewDate { get; set; }

        public string? CVUrl { get; set; }

        public string? Note { get; set; }
    }
}