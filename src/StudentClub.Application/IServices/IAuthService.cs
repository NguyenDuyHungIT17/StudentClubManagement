using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.IServices
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<bool> SendPasswordResetCodeAsync(string email);
        bool VerifyPasswordResetCode(string email, string code);
        Task<bool> ResetPasswordAsync(string email,string code, string newPassword);
    }
}
