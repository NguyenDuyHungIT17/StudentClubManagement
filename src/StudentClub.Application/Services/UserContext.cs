using Microsoft.AspNetCore.Http;
using StudentClub.Application.IServices;
using System.Security.Claims;

namespace StudentClub.Application.Services
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                var userIdStr = _httpContextAccessor.HttpContext?.User
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                return int.TryParse(userIdStr, out int id) ? id : 0;
            }
        }

        public string Role
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User
                    .FindFirst(ClaimTypes.Role)?.Value ?? "";
            }
        }
    }
}