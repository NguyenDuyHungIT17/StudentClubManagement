using FluentValidation;
using StudentClub.Application.DTOs.Request.Interview;

namespace StudentClub.API.Validators.Interview
{
    public class CreateInterviewValidator : AbstractValidator<CreateInterviewRequestDto>
    {
        public CreateInterviewValidator()
        {
            // ClubId bắt buộc
            RuleFor(x => x.ClubId)
                .GreaterThan(0)
                .WithMessage("ClubId không hợp lệ");

            // ApplicantName bắt buộc
            RuleFor(x => x.ApplicantName)
                .NotEmpty()
                .WithMessage("Tên ứng viên không được để trống")
                .MaximumLength(100)
                .WithMessage("Tên ứng viên không được vượt quá 100 ký tự");

            // Email nếu có thì phải đúng format (không bắt buộc)
            RuleFor(x => x.ApplicantEmail)
                .EmailAddress()
                .WithMessage("Email không đúng định dạng")
                .When(x => !string.IsNullOrWhiteSpace(x.ApplicantEmail));

            // Phone max length
            RuleFor(x => x.ApplicantPhone)
                .MaximumLength(20)
                .WithMessage("Số điện thoại không được vượt quá 20 ký tự")
                .Matches(@"^\d+$")
                .WithMessage("Số điện thoại chỉ chứa chữ số")
                .When(x => !string.IsNullOrWhiteSpace(x.ApplicantPhone));

            // InterviewDate không được là quá khứ (nếu có)
            RuleFor(x => x.InterviewDate)
                .GreaterThanOrEqualTo(DateTime.Now.Date)
                .WithMessage("Thời gian phỏng vấn không hợp lệ")
                .When(x => x.InterviewDate.HasValue);

            // CVUrl max length
            RuleFor(x => x.CVUrl)
                .MaximumLength(255)
                .WithMessage("CVUrl không được vượt quá 255 ký tự");

            // Note max length
            RuleFor(x => x.Note)
                .MaximumLength(500)
                .WithMessage("Ghi chú không được vượt quá 500 ký tự");
        }
    }

    public class StartInterviewValidator : AbstractValidator<StartInterviewRequestDto>
    {
        public StartInterviewValidator()
        {
            RuleFor(x => x.EvaluatorId)
                .GreaterThan(0)
                .WithMessage("EvaluatorId không hợp lệ");

            RuleFor(x => x.EvaluatorName)
                .NotEmpty()
                .WithMessage("Tên người phỏng vấn không được để trống");
        }
    }

    public class FinishInterviewValidator : AbstractValidator<FinishInterviewRequestDto>
    {
        public FinishInterviewValidator()
        {
            RuleFor(x => x.Evaluation)
                .MaximumLength(500)
                .WithMessage("Đánh giá không được vượt quá 500 ký tự");
        }
    }

    public class UpdateInterviewAfterInterviewValidator : AbstractValidator<UpdateInterviewAfterInterview>
    {
        public UpdateInterviewAfterInterviewValidator()
        {
            RuleFor(x => x.Evaluation)
                .MaximumLength(500)
                .WithMessage("Đánh giá không được vượt quá 500 ký tự");
        }
    }
}