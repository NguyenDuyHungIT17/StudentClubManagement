using System;

namespace StudentClub.Application.DTOs.response.Event
{
    public class GetAllEventsResponseDto
    {
        public int Id { get; set; }
        public int ClubId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? Priority { get; set; }
        public bool? IsPrivate { get; set; }
        public DateTime? EventDate { get; set; }

        /// <summary>
        /// Main event photo URL (prioritize PhotoType.Main, fallback to first uploaded photo)
        /// </summary>
        public string? PhotoUrl { get; set; }
    }
}