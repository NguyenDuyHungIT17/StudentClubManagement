using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.Filter
{
    public class EventFilterRequest : BaseFilter
    {
        public string? Keyword { get; set; }
        public int ClubId { get; set; }
        public bool? IsPrivate { get; set; }
        public int? Priority { get; set; }
    }
}
