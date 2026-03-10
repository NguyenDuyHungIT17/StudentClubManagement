namespace StudentClub.Application.DTOs.response.ClubMember
{
    public class CreateClubMemberResponseDto
    {
        public int ClubMemberId { get; set; }
        public string ClubName { get; set; }

        public string UserName { get; set; }

        public string MemberRole { get; set; }

        public DateTime? JoinAt { get; set; }
    }
}
