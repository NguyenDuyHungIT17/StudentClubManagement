using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.Clubs
{
    public class CreateClubResponseDto
    {
        public string ClubName { get; set; }
        public string Description { get; set; }
        public string LeaderName { get; set; }
    }
}
