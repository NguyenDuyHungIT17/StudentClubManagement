namespace StudentClub.Application.DTOs.response.EventRegistration
{
    public class CreateEventRegistrationResponseDto 
    {
        public int Id { get; set; }
        public int EventId { get; set; }

        public int? UserId { get; set; }

        public bool? CheckedIn { get; set; }

        public string? CheckName { get; set; }

        public DateTime? RegisteredAt { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? GuestEmail { get; set; }

        public string? GuestName { get; set; }

        public int? IsCare { get; set; }

        public int? CLubId { get; set; }
    }
}
