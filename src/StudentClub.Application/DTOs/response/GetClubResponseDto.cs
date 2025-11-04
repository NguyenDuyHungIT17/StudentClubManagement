using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.response
{
    public class GetClubResponseDto
    {
        public int ClubId { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string LeaderName { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }
    }
}
