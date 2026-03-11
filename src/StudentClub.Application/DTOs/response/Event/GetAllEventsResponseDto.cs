using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.response.Event
{
    public class GetAllEventsResponseDto
    {
        public int Id { get; set; }
        public string ClubName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? Priority { get; set; }
        public DateTime? EventDate { get; set; }
    }
}
