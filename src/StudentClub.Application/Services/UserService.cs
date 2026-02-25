using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Domain.Entities;
using StudentClub.Shared.ApiResponse; // Thêm namespace này
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentClub.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IClubMemberRepository _clubMemberRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UserService> _logger;

        public UserService(IClubRepository clubRepository, IUserRepository userRepository, IClubMemberRepository clubMemberRepository, IPasswordHasher passwordHasher, ILogger<UserService> logger)
        {
            _clubMemberRepository = clubMemberRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _clubRepository = clubRepository;
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

        public async Task<ApiResponse<CreateUserResponseDto>> CreateUserAsync(CreateUserRequestDto createUserRequset)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(createUserRequset.Email);
                if (existingUser != null)
                {
                    return ApiResponse<CreateUserResponseDto>.Failure(400, "Account already exist");
                }

                var user = new User
                {
                    FullName = createUserRequset.FullName,
                    Email = createUserRequset.Email,
                    PasswordHash = _passwordHasher.Hash(createUserRequset.Password),
                    Role = createUserRequset.Role,
                    IsActive = createUserRequset.IsActive,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangeAsynce();

                var newclubMember = new ClubMember
                {
                    ClubId = createUserRequset.ClubId,
                    UserId = user.UserId,
                    MemberRole = createUserRequset.Role,
                    JoinedAt = DateTime.UtcNow,
                };
                await _clubMemberRepository.AddClubMemberAsync(newclubMember);
                await _clubMemberRepository.SaveChangeAsync();

                var result = new CreateUserResponseDto
                {
                    Email = user.Email,
                    Role = user.Role,
                    ClubName = await _clubRepository.GetCLubNameByClubIdAsync(createUserRequset.ClubId)
                };

                return ApiResponse<CreateUserResponseDto>.Success(result, "Tạo người dùng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo user. Email: {Email}", createUserRequset.Email);
                return ApiResponse<CreateUserResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<UpdateUserResponseDto>> UpdateUserAsync(int userIdFromToken, string role, int targetUserId, UpdateUserRequestDto request)
        {
            try
            {
                var user = await _userRepository.GetUserByUserIdAsync(targetUserId);
                if (user == null)
                    return ApiResponse<UpdateUserResponseDto>.Failure(404, "User does not exist");

                if (role == "member" && userIdFromToken != targetUserId)
                    return ApiResponse<UpdateUserResponseDto>.Failure(403, "Bạn không có quyền chỉnh sửa user này.");

                if (request.isActive != 0 && request.isActive != 1)
                    return ApiResponse<UpdateUserResponseDto>.Failure(400, "Giá trị IsActive chỉ có thể là 0 hoặc 1.");

                user.FullName = string.IsNullOrWhiteSpace(request.FullName) ? user.FullName : request.FullName;
                user.Email = string.IsNullOrWhiteSpace(request.Email) ? user.Email : request.Email;
                user.Role = string.IsNullOrWhiteSpace(request.Role) ? user.Role : request.Role;
                user.UpdatedAt = DateTime.UtcNow;
                user.IsActive = request.isActive;

                await _userRepository.UpdateUserAsync(user);
                await _userRepository.SaveChangeAsynce();

                var result = new UpdateUserResponseDto
                {
                    FullName = user.FullName,
                    Role = user.Role,
                    Email = user.Email,
                    IsActive = user.IsActive
                };

                return ApiResponse<UpdateUserResponseDto>.Success(result, "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật user. UserId: {UserId}", targetUserId);
                return ApiResponse<UpdateUserResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteUserAsync(int requesterId, string requesterRole, int targetUserId)
        {
            try
            {
                var user = await _userRepository.GetUserByUserIdAsync(targetUserId);
                if (user == null)
                    return ApiResponse.Failure(404, "User không tồn tại.");

                if (requesterRole == "admin")
                {
                    await _userRepository.DeleteUserAsync(user);
                    await _userRepository.SaveChangeAsynce();
                    return ApiResponse.Success("Xóa người dùng thành công (Admin)");
                }

                if (requesterRole == "leader")
                {
                    if (user.Role != "member")
                        return ApiResponse.Failure(403, "Leader chỉ có thể xóa user là member.");

                    await _userRepository.DeleteUserAsync(user);
                    await _userRepository.SaveChangeAsynce();
                    return ApiResponse.Success("Xóa thành viên thành công (Leader)");
                }

                return ApiResponse.Failure(403, "Bạn không có quyền xóa user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa user. UserId: {UserId}", targetUserId);
                return ApiResponse.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse> UpdateIsActiveUserAsync(int isActive, int userId)
        {
            try
            {
                if (isActive != 0 && isActive != 1)
                    return ApiResponse.Failure(400, "Giá trị IsActive chỉ có thể là 0 hoặc 1.");

                var user = await _userRepository.GetUserByUserIdAsync(userId);
                if (user == null) return ApiResponse.Failure(404, "Người dùng không tồn tại");

                user.IsActive = isActive;
                await _userRepository.SaveChangeAsynce();

                return ApiResponse.Success("Cập nhật trạng thái thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật trạng thái hoạt động UserId: {UserId}", userId);
                return ApiResponse.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<GetAllUsersResponseDto>>> GetAllUsersAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetUserByUserIdAsync(id);
                if (user == null) return ApiResponse<List<GetAllUsersResponseDto>>.Failure(404, "Người yêu cầu không tồn tại");

                var users = await _userRepository.GetAllUsersAsync();
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
                else if (user.Role == "leader")
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

                return ApiResponse<List<GetAllUsersResponseDto>>.Success(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tất cả user.");
                return ApiResponse<List<GetAllUsersResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<GetUserResponseDto>> GetUserByIdAsync(int userId, string roleUser, int userIdOnToken)
        {
            try
            {
                var user = await _userRepository.GetUserByUserIdAsync(userId);
                if (user == null) return ApiResponse<GetUserResponseDto>.Failure(404, "Người dùng không tồn tại");

                if (roleUser == "admin")
                {
                    return ApiResponse<GetUserResponseDto>.Success(MapToDto(user));
                }

                if (roleUser == "leader")
                {
                    if (user.Role == "admin") return ApiResponse<GetUserResponseDto>.Failure(403, "Không có quyền xem thông tin Admin");

                    if (user.Role == "leader" && userId == userIdOnToken)
                    {
                        return ApiResponse<GetUserResponseDto>.Success(MapToDto(user));
                    }

                    var clubId = await _clubMemberRepository.GetClubIdByUserId(userIdOnToken);
                    var usersInClub = await _userRepository.GetUserByLeader(clubId);

                    if (usersInClub.Any(u => u.UserId == userId))
                    {
                        return ApiResponse<GetUserResponseDto>.Success(MapToDto(user));
                    }
                }

                if (roleUser == "member" && userId == userIdOnToken)
                {
                    return ApiResponse<GetUserResponseDto>.Success(MapToDto(user));
                }

                return ApiResponse<GetUserResponseDto>.Failure(403, "Bạn không có quyền truy cập thông tin này.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy user theo ID: {UserId}", userId);
                return ApiResponse<GetUserResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse> UpdatePasswordUserAsync(int userIdOnToken, int userId, string oldPassword, string newPassword)
        {
            try
            {
                if (userIdOnToken != userId)
                    return ApiResponse.Failure(403, "Bạn không có quyền thay đổi mật khẩu của người dùng khác.");

                if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
                    return ApiResponse.Failure(400, "Mật khẩu cũ và mật khẩu mới không được để trống.");

                if (oldPassword == _passwordHasher.Hash(newPassword))
                    return ApiResponse.Failure(400, "Mật khẩu mới phải khác mật khẩu cũ.");

                await _userRepository.UpdatePasswordAsync(userId, _passwordHasher.Hash(newPassword));
                return ApiResponse.Success("Cập nhật mật khẩu thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật mật khẩu UserId: {UserId}", userId);
                return ApiResponse.Failure(500, ex.Message);
            }
        }
    }
}