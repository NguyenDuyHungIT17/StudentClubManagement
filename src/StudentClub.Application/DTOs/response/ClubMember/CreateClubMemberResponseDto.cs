using System;

namespace StudentClub.Application.DTOs.response.ClubMember
{
    public class CreateClubMemberResponseDto
    {
        public int ClubMemberId { get; set; }
        public int ClubId { get; set; }

        public int UserId { get; set; }
        public string MemberRole { get; set; } = string.Empty;

        public DateTime? JoinAt { get; set; }

        // Main photo URL for this club member (prioritize Main, fallback to first)
        public string? PhotoUrl { get; set; }
    }
}