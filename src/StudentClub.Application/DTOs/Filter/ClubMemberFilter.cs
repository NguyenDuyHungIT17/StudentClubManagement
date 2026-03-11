using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.Filter
{
    public class ClubMemberFilter : BaseFilter
    {
        public int? ClubId { get; set; }
        public string? MemberRole { get; set; }
    }
}
