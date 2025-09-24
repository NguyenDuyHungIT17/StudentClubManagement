using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs
{
    public class UpdateInterviewRequestDto
    {
        public string? Evaluation { get; set; }
        public string Result { get; set; } = "Pending"; // Fail | Pass | Pending
    }

}
