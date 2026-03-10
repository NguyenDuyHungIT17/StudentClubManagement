using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.request.Club
{
    public class CreateClubRequestDto
    {
        public string ClubName { get; set; } = null!;

        public string? Title { get; set; }

        public string? Description { get; set; }

        public int? LeaderId { get; set; }

    }
}
