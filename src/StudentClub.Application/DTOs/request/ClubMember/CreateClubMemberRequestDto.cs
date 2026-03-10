namespace StudentClub.Application.DTOs.request.ClubMember
{
    public class CreateClubMemberRequestDto
    {
        public int ClubId { get; set; }
        public int UserId   { get; set; }
        public string MemberRole { get; set; }

        public DateTime JoinAt { get; set; }
    }
}
