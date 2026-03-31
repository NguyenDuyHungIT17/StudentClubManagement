using StudentClub.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentClub.Domain.Entities
{
    [Table("Photos")]
    public class Photo
    {
        [Key]
        public int PhotoId { get; set; }

        [Required]
        public string Url { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PublicId { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public PhotoType Type { get; set; } 

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? UserId { get; set; }

        public int? ClubId { get; set; }

        public int? EventId { get; set; }

        public int? ClubMemberId { get; set; }

        public int? CampaignsId { get; set; }
    }
}