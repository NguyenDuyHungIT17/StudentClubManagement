using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Domain.Entities;

namespace StudentClub.Application.Services
{
    public class ClubMemberService : IClubMemberService
    {
        private IClubMemberRepository _clubmemberRepository;
        private IUserRepository _userRepository;
        private IClubRepository _clubRepository;
        private readonly ILogger<ClubMemberService> _logger;

        public ClubMemberService(IClubMemberRepository clubmemberRepository, IUserRepository userRepository, IClubRepository clubRepository, ILogger<ClubMemberService> logger)
        {
            _clubmemberRepository = clubmemberRepository;
            _userRepository = userRepository;
            _clubRepository = clubRepository;
            _logger = logger;
        }

        public async Task<CreateClubMemberResponseDto> CreateClubMemberAsync(CreateClubMemberRequestDto createClubMemberRequestDto)
        {
            try
            {
                var existingClub = await _clubRepository.GetClubByClubIdAsync(createClubMemberRequestDto.ClubId);
                if (existingClub == null)
                {
                    throw new Exception("Club is not exist");
                }

                var existingUser = await _userRepository.GetUserByUserIdAsync(createClubMemberRequestDto.UserId);
                if (existingUser == null)
                {
                    throw new Exception("User is not exist");
                }

                var clubMember = new ClubMember
                {
                    ClubId = createClubMemberRequestDto.ClubId,
                    UserId = createClubMemberRequestDto.UserId,
                    JoinedAt = createClubMemberRequestDto.JoinAt,
                    MemberRole = createClubMemberRequestDto.MemberRole,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                await _clubmemberRepository.AddClubMemberAsync(clubMember);
                await _clubRepository.SaveChangeAsync();

                if (createClubMemberRequestDto.MemberRole.Equals("leader"))
                {
                    await _clubRepository.UpdateLeaderIdAsync(createClubMemberRequestDto.ClubId, createClubMemberRequestDto.UserId);
                    await _clubmemberRepository.SaveChangeAsync();
                }
                return new CreateClubMemberResponseDto
                {
                    clubName = existingClub.ClubName,
                    userName = existingUser.FullName,
                    MemberRole = clubMember.MemberRole,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm thành viên vào câu lạc bộ. UserId: {UserId}, ClubId: {ClubId}, Thời gian: {Time}", createClubMemberRequestDto.UserId, createClubMemberRequestDto.ClubId, DateTime.UtcNow);
                throw;

            }
        }
    }
}
