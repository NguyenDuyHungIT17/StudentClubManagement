using FluentValidation;
using StudentClub.Application.DTOs.request.User;

namespace StudentClub.API.Validators.User
{
    public class CreateUserRequest
        : AbstractValidator<CreateUserRequestDto>
    {
        public CreateUserRequest()
        {
            // FullName
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên không được để trống")
                .MaximumLength(100).WithMessage("Họ tên tối đa 100 ký tự");

            // Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không hợp lệ")
                .MaximumLength(150).WithMessage("Email tối đa 150 ký tự");

            // Password
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(6).WithMessage("Mật khẩu tối thiểu 6 ký tự")
                .MaximumLength(100).WithMessage("Mật khẩu tối đa 100 ký tự");

            // ClubId
            RuleFor(x => x.ClubId)
                .GreaterThan(0).WithMessage("ClubId phải lớn hơn 0");

            // Role
            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role không được để trống")
                .Must(role => role == "admin" || role == "leader" || role == "member")
                .WithMessage("Role không hợp lệ");

            // IsActive
            RuleFor(x => x.IsActive)
                .InclusiveBetween(0, 1)
                .WithMessage("IsActive chỉ nhận giá trị 0 hoặc 1");
        }
    }
}
