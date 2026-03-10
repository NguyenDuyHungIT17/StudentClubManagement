namespace StudentClub.Application.DTOs.response.User
{
    public class UpdateUserResponseDto
    {

        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; }=string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? IsActive { get; set; }

    }
}
