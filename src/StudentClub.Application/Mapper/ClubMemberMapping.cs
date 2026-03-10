using StudentClub.Application.DTOs.request.ClubMember;
using StudentClub.Application.DTOs.response.ClubMember;
using StudentClub.Domain.Entities;

namespace StudentClub.Application.Mapper
{
    public class ClubMemberMapping
    {
        public async virtual Task<CreateClubMemberResponseDto> ToResponse(ClubMember clubMember)
        {
            return new CreateClubMemberResponseDto
            {
                ClubMemberId = clubMember.ClubMemberId,
                ClubName = clubMember.Club.ClubName,
                UserName = clubMember.User.FullName,
                MemberRole = clubMember.MemberRole,
                JoinAt = clubMember.JoinedAt,
            };
        }

        public async virtual Task<List<CreateClubMemberResponseDto>> ToDtoList(List<ClubMember> clubmembers)
        {
            var result = new List<CreateClubMemberResponseDto>();
            foreach (var item in clubmembers)
            {
                var dto = await ToResponse(item);
                result.Add(dto);
            }
            return result;
        }

        public async virtual Task<ClubMember> ToEntity(CreateClubMemberRequestDto request)
        {
            return new ClubMember
            {
                ClubId = request.ClubId,
                UserId = request.UserId,
                JoinedAt = request.JoinAt,
            };
        }
    }
}
