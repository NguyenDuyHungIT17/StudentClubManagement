using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.response
{
    public class UpdateClubResponseDto
    {
        public string ClubName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int LeaderId {get; set; }
    }
}
