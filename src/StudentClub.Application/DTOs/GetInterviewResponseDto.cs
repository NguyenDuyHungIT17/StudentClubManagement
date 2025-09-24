using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs
{
    public class GetInterviewResponseDto
    {
        public int InterviewId { get; set; }
        public int ClubId { get; set; }
        public string ApplicantName { get; set; } = null!;
        public string ApplicantEmail { get; set; } = null!;
        public string? Evaluation { get; set; }
        public string Result { get; set; } = "Pending";
        public DateTime? CreatedAt { get; set; }
    }

}
