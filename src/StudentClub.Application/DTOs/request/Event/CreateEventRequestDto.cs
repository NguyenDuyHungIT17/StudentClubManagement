namespace StudentClub.Application.DTOs.request.Event
{
    public class CreateEventRequestDto
    {
        public int ClubId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Location { get; set; }
        public bool IsPrivate { get; set; }
        public int? Priority { get; set; }
    }
}
