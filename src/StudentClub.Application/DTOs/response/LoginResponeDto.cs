using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.response
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}
