using FluentValidation;
using StudentClub.Application.DTOs.request.EventRegistration;

namespace StudentClub.API.Validators.EventRegistration
{
    public class CreateEventRegistration : AbstractValidator<CreateEventRegistrationRequestDto>
    {
        public CreateEventRegistration()
        {
            // EventId bắt buộc
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("EventId không được để trống");

            RuleFor(x => x.CheckName)
                .MaximumLength(50)
                .WithMessage("Ghi chú không được vượt quá 50 kí tự");

            // UserId >= 0
            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(0)
                .WithMessage("UserId không hợp lệ");

            // GuestEmail: bắt buộc nếu UserId = 0
            RuleFor(x => x.GuestEmail)
                .NotEmpty()
                .WithMessage("Email không được để trống")
                .EmailAddress()
                .WithMessage("Email không đúng định dạng")
                .When(x => x.UserId == 0);

            // GuestName: bắt buộc nếu UserId = 0
            RuleFor(x => x.GuestName)
                .NotEmpty()
                .WithMessage("Tên khách không được để trống")
                .MaximumLength(100)
                .WithMessage("Tên khách không được vượt quá 100 ký tự")
                .When(x => x.UserId == 0);

            // Nếu có UserId thì không được nhập GuestEmail (optional - strict)
            RuleFor(x => x)
                .Must(x => !(x.UserId > 0 && !string.IsNullOrWhiteSpace(x.GuestEmail)))
                .WithMessage("Không được nhập GuestEmail khi đã có UserId");

            // Nếu có UserId thì không được nhập GuestName (optional - strict)
            RuleFor(x => x)
                .Must(x => !(x.UserId > 0 && !string.IsNullOrWhiteSpace(x.GuestName)))
                .WithMessage("Không được nhập GuestName khi đã có UserId");
        }
    }
}