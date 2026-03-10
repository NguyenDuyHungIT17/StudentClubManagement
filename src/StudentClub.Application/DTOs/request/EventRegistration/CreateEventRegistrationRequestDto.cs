namespace StudentClub.Application.DTOs.request.EventRegistration
{
    public class CreateEventRegistrationRequestDto
    {
        public int EventId { get; set; }

        public int UserId { get; set; }

        public bool? CheckedIn { get; set; }

        public string? CheckName { get; set; }

    }
}
