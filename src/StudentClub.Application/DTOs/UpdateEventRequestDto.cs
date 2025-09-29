using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs
{
    public class UpdateEventRequestDto
    {
        public int ClubId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
