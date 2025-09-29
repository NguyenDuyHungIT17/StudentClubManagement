using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs
{
    public class GetAllUsersResponseDto
    {
        public int userId {  get; set; }    
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
        public int? IsActive {get; set; }
    }
}
