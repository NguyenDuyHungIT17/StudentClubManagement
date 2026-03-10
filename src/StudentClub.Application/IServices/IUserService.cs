using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.User;
using StudentClub.Application.DTOs.response.User;
using StudentClub.Shared.ApiResponse; // Thêm namespace này

namespace StudentClub.Application.IServices
{
    public interface IUserService
    {
        Task<ApiResponse<CreateUserResponseDto>> CreateUserAsync(CreateUserRequestDto createUserRequset);
        Task<ApiResponse<UpdateUserResponseDto>> UpdateUserAsync(int userIdFromToken, string role, int targetUserId, UpdateUserRequestDto request);
        Task<ApiResponse> UpdateIsActiveUserAsync(int isActive, int userId);
        Task<ApiResponse> UpdatePasswordUserAsync(int userIdOnToken, int userId, string oldPassword, string newPassword);
        Task<ApiResponse> DeleteUserAsync(int requesterId, string requesterRole, int targetUserId);
        Task<PagedResponse<GetAllUsersResponseDto>> GetAllUsersAsync(int id, UserFilterRequest filter);
        Task<ApiResponse<GetUserResponseDto>> GetUserByIdAsync(int userId, string roleUser, int userIdOnToken);
    }
}