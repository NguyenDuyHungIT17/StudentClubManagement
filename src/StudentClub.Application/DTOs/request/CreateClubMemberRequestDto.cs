using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.request
{
    public class CreateClubMemberRequestDto
    {
        public int ClubId { get; set; }
        public int UserId   { get; set; }
        public string MemberRole { get; set; }

        public DateTime JoinAt { get; set; }
    }
}
