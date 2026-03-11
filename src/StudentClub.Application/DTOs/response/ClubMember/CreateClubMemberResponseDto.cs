namespace StudentClub.Application.DTOs.response.ClubMember
{
    public class CreateClubMemberResponseDto
    {
        public int ClubMemberId { get; set; }
        public int ClubId { get; set; }

        public int UserId { get; set; }
        public string MemberRole { get; set; }

        public DateTime? JoinAt { get; set; }
    }
}
