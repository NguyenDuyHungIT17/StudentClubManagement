using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.User;
using StudentClub.Application.DTOs.response.User;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Domain.Entities;
using StudentClub.Shared.ApiResponse;
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
        private readonly IPhotoService _photoService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IClubRepository clubRepository,
            IUserRepository userRepository,
            IClubMemberRepository clubMemberRepository,
            IPasswordHasher passwordHasher,
            IPhotoService photoService,
            ILogger<UserService> logger)
        {
            _clubMemberRepository = clubMemberRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _clubRepository = clubRepository;
            _photoService = photoService;
            _logger = logger;
        }

        private GetUserResponseDto MapToDto(User user)
        {
            return new GetUserResponseDto
            {
                UserId = user.UserId,
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

                var clubExisting = await _clubRepository.GetClubByClubIdAsync(createUserRequset.ClubId);

                if (clubExisting == null)
                {
                    return ApiResponse<CreateUserResponseDto>.Failure(400, "Câu lạc bộ không tồn tại");
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
                    UserId = user.UserId,
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

        public async Task<PagedResponse<GetAllUsersResponseDto>> GetAllUsersAsync(int id, UserFilterRequest filter)
        {
            try
            {
                var caller = await _userRepository.GetUserByUserIdAsync(id);
                if (caller == null) throw new Exception("Caller not found");

                // Build queryable and apply role-based scope BEFORE materializing
                var q = _userRepository.QueryUsers();

                if (caller.Role == "leader")
                {
                    var clubId = await _clubMemberRepository.GetClubIdByUserId(id);
                    // get member ids for the leader's club
                    var memberIds = await _clubMemberRepository.QueryClubMembers()
                        .Where(cm => cm.ClubId == clubId)
                        .Select(cm => cm.UserId)
                        .ToListAsync();

                    q = q.Where(u => memberIds.Contains(u.UserId));
                }
                // if admin, keep all users

                // Apply filters at DB level
                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    var keyword = filter.KeyWord.Trim().ToLower();
                    q = q.Where(u => (u.Email != null && u.Email.ToLower().Contains(keyword)) ||
                                     (u.FullName != null && u.FullName.ToLower().Contains(keyword)));
                }

                if (!string.IsNullOrWhiteSpace(filter.Role))
                {
                    var role = filter.Role.Trim().ToLower();
                    q = q.Where(u => u.Role != null && u.Role.ToLower() == role);
                }

                if (filter.IsActive.HasValue)
                {
                    q = q.Where(u => u.IsActive == filter.IsActive);
                }

                // Count before paging
                var total = await q.CountAsync();

                var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
                var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

                // Project only needed fields and page in DB
                var projected = await q
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new GetAllUsersResponseDto
                    {
                        userId = u.UserId,
                        Email = u.Email,
                        FullName = u.FullName,
                        Role = u.Role,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt
                    })
                    .ToListAsync();

                // Batch load photos for the page
                var userIds = projected.Select(u => u.userId).ToList();
                var photoMap = userIds.Count > 0
                    ? await _photoService.GetMainPhotoUrlsByUserIdsAsync(userIds)
                    : new Dictionary<int, string?>();

                foreach (var dto in projected)
                {
                    dto.PhotoUrl = photoMap.ContainsKey(dto.userId) ? photoMap[dto.userId] : null;
                }

                var totalPages = (int)Math.Ceiling(total / (double)pageSize);

                return new PagedResponse<GetAllUsersResponseDto>
                {
                    Items = projected,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalCount = total
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tất cả user.");
                throw;
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
                    var dto = MapToDto(user);
                    dto.PhotoUrl = await _photoService.GetMainPhotoUrlAsync(userId, null, null, null);
                    return ApiResponse<GetUserResponseDto>.Success(dto);
                }

                if (roleUser == "leader")
                {
                    if (user.Role == "admin") return ApiResponse<GetUserResponseDto>.Failure(403, "Không có quyền xem thông tin Admin");

                    if (user.Role == "leader" && userId == userIdOnToken)
                    {
                        var dto = MapToDto(user);
                        dto.PhotoUrl = await _photoService.GetMainPhotoUrlAsync(userId, null, null, null);
                        return ApiResponse<GetUserResponseDto>.Success(dto);
                    }

                    var clubId = await _clubMemberRepository.GetClubIdByUserId(userIdOnToken);
                    var usersInClub = await _userRepository.GetUserByLeader(clubId);

                    if (usersInClub.Any(u => u.UserId == userId))
                    {
                        var dto = MapToDto(user);
                        dto.PhotoUrl = await _photoService.GetMainPhotoUrlAsync(userId, null, null, null);
                        return ApiResponse<GetUserResponseDto>.Success(dto);
                    }
                }

                if (roleUser == "member" && userId == userIdOnToken)
                {
                    var dto = MapToDto(user);
                    dto.PhotoUrl = await _photoService.GetMainPhotoUrlAsync(userId, null, null, null);
                    return ApiResponse<GetUserResponseDto>.Success(dto);
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

        private void _club_repository_check(IClubRepository repo) { }

    }
}