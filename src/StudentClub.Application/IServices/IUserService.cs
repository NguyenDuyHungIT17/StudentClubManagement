using StudentClub.Application.DTOs;
using StudentClub.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.IServices
{
    public interface IUserService
    {
        Task<CreateUserResponseDto> CreateUserAsync(CreateUserRequestDto createUserRequset);
        Task<UpdateUserResponseDto> UpdateUserAsync(int userIdFromToken, string role, int targetUserId, UpdateUserRequestDto request);
        Task UpdateIsActiveUserAsync(int isActive, int userId);
        Task DeleteUserAsync(int requesterId, string requesterRole, int targetUserId);


    }
}
