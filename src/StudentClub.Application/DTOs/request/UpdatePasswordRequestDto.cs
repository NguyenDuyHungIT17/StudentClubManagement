using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.request
{
    public class UpdatePasswordRequestDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

    }
}
