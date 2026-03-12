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
        public int ClubId { get; set; } 
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? Priority { get; set; }
        public bool? IsPrivate { get; set; }
        public DateTime? EventDate { get; set; }
    }
}
