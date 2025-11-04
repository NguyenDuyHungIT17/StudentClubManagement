using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.response
{
    public class CreateFeedbackResponseDto
    {
        public string EventName { get; set; }
        public string? Comment { get; set; }
        public int? Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
