using FluentValidation;
using StudentClub.Application.DTOs.request;

namespace StudentClub.API.Validators.Club
{
    public class UpdateClubRequest : AbstractValidator<UpdateClubRequestDto>
    {
        public UpdateClubRequest()
        {
            RuleFor(x => x.ClubName)
                .NotEmpty().WithMessage("Tên câu lạc bộ không được để trống")
                .MaximumLength(100).WithMessage("Tên câu lạc bộ tối đa 100 ký tự");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề không được để trống")
                .MaximumLength(150)
                .WithMessage("Tiêu đề tối đa 150 ký tự")
                .When(x => !string.IsNullOrWhiteSpace(x.Title));

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Mô tả tối đa 1000 ký tự")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.LeaderId)
                .NotNull().WithMessage("LeaderId là bắt buộc")
                .GreaterThan(0).WithMessage("LeaderId phải lớn hơn 0");
        }
    }
}
