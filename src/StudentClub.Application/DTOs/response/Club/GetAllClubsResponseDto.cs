using System;

namespace StudentClub.Application.DTOs.response.Club
{
    public class GetAllClubsResponseDto
    {
        public int ClubId { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

        public string? LeaderName { get; set; } = string.Empty;
        public string? Title { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // New: main photo url (prioritize PhotoType.Main; fallback to first)
        public string? PhotoUrl { get; set; }
    }
}