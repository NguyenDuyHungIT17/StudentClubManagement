using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs
{
    public class UpdateUserRequestDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public int isActive { get; set; }
    }
}
