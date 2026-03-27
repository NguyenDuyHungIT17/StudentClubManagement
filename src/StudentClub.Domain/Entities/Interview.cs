using StudentClub.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentClub.Domain.Entities
{
    [Table("Interviews")]
    public class Interview
    {
        [Key]
        public int InterviewId { get; set; }

        [Required]
        public int ClubId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ApplicantName { get; set; } = null!;

        [MaxLength(100)]
        public string? ApplicantEmail { get; set; }

        [MaxLength(20)]
        public string? ApplicantPhone { get; set; }


        public DateTime? InterviewDate { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }


        [MaxLength(500)]
        public string? Evaluation { get; set; }

        public int? EvaluatorId { get; set; }

        [MaxLength(100)]
        public string? EvaluatorName { get; set; }

        [MaxLength(255)]
        public string? CVUrl { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        /// <summary>
        /// 0 Registered
        /// 1 CheckedIn
        /// 2 Interviewing
        /// 3 Done
        /// 4 NoShow
        /// 5 Cancelled
        /// </summary>
        public InterviewStatus Status { get; set; } = InterviewStatus.Registered;

        /// <summary>
        /// 0 Pending
        /// 1 Pass
        /// 2 Fail
        /// </summary>
        public InterviewResult Result { get; set; } = InterviewResult.Pending;

        /// <summary>
        /// 0 = Online, 1 = Walk-in
        /// </summary>
        public ApplicationType ApplicationType { get; set; } = ApplicationType.Online;

        public int? CampaignId { get; set; }

        [ForeignKey("CampaignId")]
        public virtual Campaigns? Campaign { get; set; }

        // Navigation
        [ForeignKey("ClubId")]
        public virtual Club Club { get; set; } = null!;
    }
}