using Microsoft.IdentityModel.JsonWebTokens;
using StudentClub.Application.DTOs;
using StudentClub.Application.DTOs.User;
using StudentClub.Application.Interfaces;
using StudentClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using StudentClub.Application.IServices;


namespace StudentClub.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IClubMemberRepository _clubMemberRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IClubMemberRepository clubMemberRepository,  IPasswordHasher passwordHasher)
        {
            _clubMemberRepository = clubMemberRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }
        //map user -> getuserResponseDto
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

        public async Task<UpdateUserResponseDto> UpdateUserAsync(int userIdFromToken, string role, int targetUserId, UpdateUserRequestDto request)
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

        public async Task DeleteUserAsync(int requesterId, string requesterRole, int targetUserId)
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

        public async Task UpdateIsActiveUserAsync(int isActive, int userId)
        {
            if (isActive != 0 && isActive != 1)
                throw new ArgumentException("Giá trị IsActive chỉ có thể là 0 hoặc 1.");

            var user = await _userRepository.GetUserByUserIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("người dùng không tồn tại");

            user.IsActive = isActive;

            await _userRepository.SaveChangeAsynce();
        }

        public async Task<List<GetAllUsersResponseDto>> GetAllUsersAsync(int id)
        {
            var user = await _userRepository.GetUserByUserIdAsync(id);
            var users =  await _userRepository.GetAllUsersAsync();
            if (users == null) throw new KeyNotFoundException("không có người dùng, hoặc bạn không đủ quyền");

            var userDtos = new List<GetAllUsersResponseDto>();
            if ( user.Role == "admin")
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

        public async Task<GetUserResponseDto?> GetUserByIdAsync(int userId, string roleUser, int userIdOnToken)
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

    }
}
