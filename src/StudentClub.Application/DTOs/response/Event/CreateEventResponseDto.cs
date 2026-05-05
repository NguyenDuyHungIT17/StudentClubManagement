using System;

namespace StudentClub.Application.DTOs.response.Event
{
    public class CreateEventResponseDto
    {
        public int Id { get; set; }
        public int ClubId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? Priority { get; set; }
        public bool? IsPrivate { get; set; }
        public DateTime? EventDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Location { get; set; }
        public bool IsFinish { get; set; }

        /// <summary>
        /// Main event photo URL (prioritize PhotoType.Main, fallback to first uploaded photo)
        /// </summary>
        public string? PhotoUrl { get; set; }
    }
}