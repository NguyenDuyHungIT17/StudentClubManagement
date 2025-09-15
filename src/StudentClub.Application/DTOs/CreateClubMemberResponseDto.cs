using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs
{
    public class CreateClubMemberResponseDto
    {
        public string clubName { get; set; }

        public string userName { get; set; }

        public string MemberRole { get; set; }
    }
}
