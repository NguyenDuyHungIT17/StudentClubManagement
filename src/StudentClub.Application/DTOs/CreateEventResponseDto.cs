using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs
{
    public class CreateEventResponseDto
    {
        public string ClubName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? EventDate { get; set; }
    }
}
