using FluentValidation;
using StudentClub.Application.DTOs.request;

namespace StudentClub.API.Validators.User
{
    public class UpdateUserRequestValidator
        : AbstractValidator<UpdateUserRequestDto>
    {
        public UpdateUserRequestValidator()
        {
            // Email
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email không hợp lệ")
                .MaximumLength(150).WithMessage("Email tối đa 150 ký tự")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            // FullName (optional)
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên không được để trống")
                .MaximumLength(100).WithMessage("Họ tên tối đa 100 ký tự")
                .When(x => !string.IsNullOrWhiteSpace(x.FullName));

            // Role (optional)
            RuleFor(x => x.Role)
                .Must(role =>
                    role == "Admin" ||
                    role == "Leader" ||
                    role == "Member"
                )
                .WithMessage("Role không hợp lệ")
                .When(x => !string.IsNullOrWhiteSpace(x.Role));

            // isActive (required)
            RuleFor(x => x.isActive)
                .InclusiveBetween(0, 1)
                .WithMessage("isActive chỉ nhận giá trị 0 hoặc 1");
        }
    }
}
