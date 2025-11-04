using Microsoft.IdentityModel.JsonWebTokens;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using StudentClub.Application.IServices;
using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;


namespace StudentClub.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IClubMemberRepository _clubMemberRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IClubMemberRepository clubMemberRepository,  IPasswordHasher passwordHasher, ILogger<UserService> logger)
        {
            _clubMemberRepository = clubMemberRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        private GetUserResponseDto MapToDto(User user)
        {
            return new GetUserResponseDto
            {
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        public async Task<CreateUserResponseDto> CreateUserAsync(CreateUserRequestDto createUserRequset)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(createUserRequset.Email);
                if (existingUser != null)
                {
                    throw new Exception("Account already exist");
                }

                var user = new User
                {
                    FullName = createUserRequset.Email,
                    Email = createUserRequset.Email,
                    PasswordHash = _passwordHasher.Hash(createUserRequset.Password),
                    Role = createUserRequset.Role,
                    IsActive = createUserRequset.IsActive,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangeAsynce();

                return new CreateUserResponseDto
                {
                    Email = user.Email,
                    Role = user.Role,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo user. Email: {Email}, Thời gian: {Time}", createUserRequset.Email, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<UpdateUserResponseDto> UpdateUserAsync(int userIdFromToken, string role, int targetUserId, UpdateUserRequestDto request)
        {
            try
            {
                var user = await _userRepository.GetUserByUserIdAsync(targetUserId);
                if (user == null)
                    throw new KeyNotFoundException("User does not exist");

                if (role == "member" && userIdFromToken != targetUserId)
                    throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa user này.");

                if (request.isActive != 0 && request.isActive != 1)
                    throw new ArgumentException("Giá trị IsActive chỉ có thể là 0 hoặc 1.");

                user.FullName = string.IsNullOrWhiteSpace(request.FullName) ? user.FullName : request.FullName;
                user.Email = string.IsNullOrWhiteSpace(request.Email) ? user.Email : request.Email;
                user.UpdatedAt = DateTime.UtcNow;
                user.IsActive = request.isActive;

                await _userRepository.SaveChangeAsynce();
                await _userRepository.UpdateUserAsync(user);
                await _userRepository.SaveChangeAsynce();

                return new UpdateUserResponseDto
                {
                    FullName = user.FullName,
                    Role = user.Role,
                    Email = user.Email,
                    IsActive = user.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật user. UserId: {UserId}, Thời gian: {Time}", targetUserId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task DeleteUserAsync(int requesterId, string requesterRole, int targetUserId)
        {
            try
            {
                var user = await _userRepository.GetUserByUserIdAsync(targetUserId);
                if (user == null)
                    throw new KeyNotFoundException("User không tồn tại.");

                if (requesterRole == "admin")
                {
                    await _userRepository.DeleteUserAsync(user);
                    await _userRepository.SaveChangeAsynce();
                    return;
                }

                if (requesterRole == "leader")
                {
                    if (user.Role != "member")
                        throw new UnauthorizedAccessException("Leader chỉ có thể xóa user là member.");

                    await _userRepository.DeleteUserAsync(user);
                    await _userRepository.SaveChangeAsynce();
                    return;
                }
                throw new UnauthorizedAccessException("Bạn không có quyền xóa user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa user. UserId: {UserId}, Thời gian: {Time}", targetUserId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task UpdateIsActiveUserAsync(int isActive, int userId)
        {
            try
            {
                if (isActive != 0 && isActive != 1)
                    throw new ArgumentException("Giá trị IsActive chỉ có thể là 0 hoặc 1.");

                var user = await _userRepository.GetUserByUserIdAsync(userId);
                if (user == null) throw new KeyNotFoundException("người dùng không tồn tại");

                user.IsActive = isActive;

                await _userRepository.SaveChangeAsynce();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái hoạt động của user. UserId: {UserId}, Thời gian: {Time}", userId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<List<GetAllUsersResponseDto>> GetAllUsersAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetUserByUserIdAsync(id);
                var users = await _userRepository.GetAllUsersAsync();
                if (users == null) throw new KeyNotFoundException("không có người dùng, hoặc bạn không đủ quyền");

                var userDtos = new List<GetAllUsersResponseDto>();
                if (user.Role == "admin")
                {
                    userDtos = users.Select(u => new GetAllUsersResponseDto
                    {
                        userId = u.UserId,
                        Email = u.Email,
                        FullName = u.FullName,
                        Role = u.Role,
                        IsActive = u.IsActive,
                    }).ToList();
                }

                if (user.Role == "leader")
                {
                    var clubId = await _clubMemberRepository.GetClubIdByUserId(id);
                    var usersLeader = await _userRepository.GetUserByLeader(clubId);
                    userDtos = usersLeader.Select(u => new GetAllUsersResponseDto
                    {
                        userId = u.UserId,
                        Email = u.Email,
                        FullName = u.FullName,
                        Role = u.Role,
                        IsActive = u.IsActive,
                    }).ToList();
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tất cả user. UserId: {UserId}, Thời gian: {Time}", id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<GetUserResponseDto?> GetUserByIdAsync(int userId, string roleUser, int userIdOnToken)
        {
            try
            {
                var user = await _userRepository.GetUserByUserIdAsync(userId);
                if (user == null) return null;

                if (roleUser == "admin")
                {
                    return MapToDto(user);
                }

                if (roleUser == "leader")
                {
                    if (user.Role == "admin") return null;

                    if (user.Role == "leader" && userId == userIdOnToken)
                    {
                        return MapToDto(user);
                    }

                    var clubId = await _clubMemberRepository.GetClubIdByUserId(userIdOnToken);
                    var usersInClub = await _userRepository.GetUserByLeader(clubId);

                    if (usersInClub.Any(u => u.UserId == userId))
                    {
                        return MapToDto(user);
                    }

                    return null;
                }

                if (roleUser == "member")
                {
                    return userId == userIdOnToken ? MapToDto(user) : null;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy user theo ID. UserId: {UserId}, Thời gian: {Time}", userId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task UpdatePasswordUserAsync(int userIdOnToken, int userId, string oldPassword, string newPassword)
        {
            if (userIdOnToken != userId)
                throw new UnauthorizedAccessException("Bạn không có quyền thay đổi mật khẩu của người dùng khác.");

            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Mật khẩu cũ và mật khẩu mới không được để trống.");
            
            if (oldPassword == _passwordHasher.Hash(newPassword))
                throw new ArgumentException("Mật khẩu mới phải khác mật khẩu cũ.");

            await _userRepository.UpdatePasswordAsync(userId, _passwordHasher.Hash(newPassword));

        }
    }
}
