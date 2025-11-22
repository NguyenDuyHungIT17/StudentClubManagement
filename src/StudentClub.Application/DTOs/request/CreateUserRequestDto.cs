namespace StudentClub.Application.DTOs.request
{
    public class CreateUserRequestDto
    {
        public string FullName { get; set; }    
        public string Email { get; set; }
        public string Password { get; set; }
        public int ClubId { get; set; } 
        public string Role { get; set; }
        public int IsActive { get; set; }
    }
}
