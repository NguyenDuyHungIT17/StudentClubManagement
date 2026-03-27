using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.DTOs.Filter
{
    public class InterviewFilter : BaseFilter
    {
        public int ClubId { get; set; }

        public int? CampaignId { get; set; }
        public int? Status { get; set; }

        public int? Result { get; set; }

        public string? Keyword { get; set; }

    }
}
