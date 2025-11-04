using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
namespace StudentClub.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailSender;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenGenerator tokenGenerator,
            IMemoryCache cache,
            IEmailService emailSender,
            ILogger<AuthService> logger
            )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
            _cache = cache;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                    return null;

                bool isValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
                if (!isValid)
                    return null;

                int isActive = await _userRepository.GetIsActiveByEmailAsync(request.Email);
                if (isActive == 0) return null;

                var token = _tokenGenerator.GenerateToken(user.UserId, user.Email, user.Role);

                return new LoginResponseDto
                {
                    Token = token,
                    FullName = user.FullName,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng nhập. Email: {Email}, Thời gian: {Time}", request.Email, DateTime.UtcNow);
                return null;
            }
        }

        public async Task<bool> SendPasswordResetCodeAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null) return false;

                var code = _passwordHasher.GenerateCode(6);
                _passwordHasher.SaveResetCode(_cache, email, code, TimeSpan.FromMinutes(15));

                var html = await _emailSender.RenderTemplateAsync("ResetPassword.html", new Dictionary<string, string>
                {
                    { "UserName", user.FullName },
                    { "Code", code }
                });
                await _emailSender.SendEmailAsync(email, "Mã xác thực đặt lại mật khẩu", html);
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi mã đặt lại mật khẩu. Email: {Email}, Thời gian: {Time}", email, DateTime.UtcNow);
                return false;
            }

        }

        public bool VerifyPasswordResetCode(string email, string code)
        {
            return _passwordHasher.VerifyResetCode(_cache, email, code);
        }

        public async Task<bool> ResetPasswordAsync(string email, string code, string newPassword)
        {
            try
            {
                if (!VerifyPasswordResetCode(email, code))
                    return false;

                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null) return false;

                user.PasswordHash = _passwordHasher.Hash(newPassword);
                await _userRepository.UpdatePasswordAsync(user.UserId, user.PasswordHash);

                // Xóa code khỏi cache sau khi dùng
                _passwordHasher.RemoveResetCode(_cache, email);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đặt lại mật khẩu. Email: {Email}, Thời gian: {Time}", email, DateTime.UtcNow);
                return false;
            }
        }
    }
}
