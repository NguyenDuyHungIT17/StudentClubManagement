using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.response.User
{
    public class CreateUserResponseDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string ClubName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
