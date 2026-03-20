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

            // Email đúng format nếu có
            RuleFor(x => x.ApplicantEmail)
                .NotEmpty()
                .WithMessage("Email không được để trống")
                .EmailAddress()
                .WithMessage("Email không đúng định dạng");

            // Phone max length
            RuleFor(x => x.ApplicantPhone)
                .MaximumLength(20)
                .WithMessage("Số điện thoại không được vượt quá 20 ký tự");

            // InterviewDate không được là quá khứ (optional nhưng nếu có thì phải đúng)
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
}