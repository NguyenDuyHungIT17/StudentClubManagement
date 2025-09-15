using StudentClub.Application.DTOs;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;

namespace StudentClub.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
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
    }
}
