using FluentValidation;
using StudentClub.Application.DTOs.request.Campaign;

namespace StudentClub.API.Validators.Campaign
{
    public class CreateCampaignValidator : AbstractValidator<CampaignRequest>
    {
        public CreateCampaignValidator()
        {
            RuleFor(x => x.ClubId)
                .GreaterThan(0)
                .WithMessage("ClubId không hợp lệ");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Tiêu đề chiến dịch không được để trống")
                .MaximumLength(255)
                .WithMessage("Tiêu đề chiến dịch không được vượt quá 255 ký tự");

            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("Ngày bắt đầu không được sau ngày kết thúc")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("Ngày kết thúc phải sau ngày bắt đầu")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
        }
    }

    public class UpdateCampaignValidator : AbstractValidator<CampaignRequest>
    {
        public UpdateCampaignValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Tiêu đề chiến dịch không được để trống")
                .MaximumLength(255)
                .WithMessage("Tiêu đề chiến dịch không được vượt quá 255 ký tự");

            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("Ngày bắt đầu không được sau ngày kết thúc")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("Ngày kết thúc phải sau ngày bắt đầu")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
        }
    }
}