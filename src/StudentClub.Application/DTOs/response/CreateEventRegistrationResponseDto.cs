using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.response
{
    public class CreateEventRegistrationResponseDto 
    {
        public string EventName { get; set; } = string.Empty;   
        public string UserName { get; set; } = string.Empty;
        public bool? CheckedIn { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public DateTime? EventDate { get; set; }
    }
}
