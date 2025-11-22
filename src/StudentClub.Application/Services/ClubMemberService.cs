using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;
using StudentClub.Domain.Entities;

namespace StudentClub.Application.Services
{
    public class ClubMemberService : IClubMemberService
    {
        private IClubMemberRepository _clubmemberRepository;
        private IUserRepository _userRepository;
        private IClubRepository _clubRepository;
        private ClubMemberMapping _clubMemberMapping;
        private readonly ILogger<ClubMemberService> _logger;

        public ClubMemberService(IClubMemberRepository clubmemberRepository, IUserRepository userRepository, IClubRepository clubRepository, ILogger<ClubMemberService> logger, ClubMemberMapping clubMemberMapping)
        {
            _clubmemberRepository = clubmemberRepository;
            _userRepository = userRepository;
            _clubRepository = clubRepository;
            _clubMemberMapping = clubMemberMapping;
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
                    ClubMemberId = clubMember.ClubMemberId,
                    ClubName = existingClub.ClubName,
                    UserName = existingUser.FullName,
                    MemberRole = clubMember.MemberRole,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm thành viên vào câu lạc bộ. UserId: {UserId}, ClubId: {ClubId}, Thời gian: {Time}", createClubMemberRequestDto.UserId, createClubMemberRequestDto.ClubId, DateTime.UtcNow);
                throw;

            }
        }

        public async Task<List<CreateClubMemberResponseDto>> GetAllClubMemberAsync()
        {
            try
            {
                var clubMembers = await _clubmemberRepository.GetAllClubMemberAsync();
                if(clubMembers == null || !clubMembers.Any())
                {
                    throw new Exception("Không có thành viên câu lạc bộ nào.");
                }

                var clubMemberDtos = await _clubMemberMapping.ToDtoList(clubMembers);

                return clubMemberDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thành viên câu lạc bộ. Thời gian: {Time}", DateTime.UtcNow);
                throw;
            }
        }

        public async Task<CreateClubMemberResponseDto> GetClubMemberByIdAsync(int id)
        {
            try
            {
                var clubMembers = await _clubmemberRepository.GetClubMemberByIdAsync(id);
                if (clubMembers == null)
                {
                    throw new Exception("Không có thành viên câu lạc bộ này.");
                }

                var clubMemberDtos = await _clubMemberMapping.ToResponse(clubMembers);

                return clubMemberDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thành viên câu lạc bộ. Thời gian: {Time}", DateTime.UtcNow);
                throw;
            }
        }

        public async Task<CreateClubMemberResponseDto> UpdateClubMemberAsync(int id, CreateClubMemberRequestDto updateClubMemberRequestDto)
        {
            try
            {
                var clubMemberTask = await _clubmemberRepository.GetClubMemberByIdAsync(id);
                if (clubMemberTask == null)
                {
                    throw new Exception("Không có thành viên câu lạc bộ này.");
                }

                clubMemberTask.ClubId = updateClubMemberRequestDto.ClubId;
                clubMemberTask.UserId = updateClubMemberRequestDto.UserId;
                clubMemberTask.JoinedAt = updateClubMemberRequestDto.JoinAt;
                clubMemberTask.MemberRole = updateClubMemberRequestDto.MemberRole;

                await _clubmemberRepository.UpdateClubMemberAsync(clubMemberTask);
                await _clubmemberRepository.SaveChangeAsync();

                var clubMemberDtos = await _clubMemberMapping.ToResponse(clubMemberTask);
                return clubMemberDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thành viên câu lạc bộ. ClubMemberId: {ClubMemberId}, Thời gian: {Time}", id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<List<CreateClubMemberResponseDto>> GetAllClubMemberByClubIdAsync(int clubId)
        {
            try
            {
                var allMembers = await _clubmemberRepository.GetAllClubMemberAsync();
                var clubMembers = allMembers.Where(cm => cm.ClubId == clubId).ToList();

                if (clubMembers == null || !clubMembers.Any())
                {
                    throw new Exception("Không có thành viên câu lạc bộ nào.");
                }

                var clubMemberDtos = await _clubMemberMapping.ToDtoList(clubMembers);

                return clubMemberDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thành viên câu lạc bộ theo ClubId. ClubId: {ClubId}, Thời gian: {Time}", clubId, DateTime.UtcNow);
                throw;
            }
        }
    }
}
