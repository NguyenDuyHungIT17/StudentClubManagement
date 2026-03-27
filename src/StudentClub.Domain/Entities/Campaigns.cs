using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Domain.Entities
{
    [Table("RecruitmentCampaigns")]
    public class Campaigns
    {
        public int CampaignId { get; set; }
        public int ClubId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
