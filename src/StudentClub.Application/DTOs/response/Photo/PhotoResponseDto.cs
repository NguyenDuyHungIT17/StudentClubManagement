using StudentClub.Domain.Enums;
using System;

namespace StudentClub.Application.DTOs.response
{
    public class PhotoResponseDto
    {
        public int PhotoId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public PhotoType Type { get; set; }
        public DateTime CreatedAt { get; set; }

        // Trả về các Id để FE biết ảnh này thuộc về đâu
        public int? UserId { get; set; }
        public int? ClubId { get; set; }
        public int? EventId { get; set; }
        public int? CampaignsId { get; set; }
    }
}