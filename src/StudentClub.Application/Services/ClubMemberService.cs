using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.ClubMember;
using StudentClub.Application.DTOs.response.ClubMember;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;
using StudentClub.Domain.Entities;
using StudentClub.Shared.ApiResponse;

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

        public async Task<ApiResponse<CreateClubMemberResponseDto>> CreateClubMemberAsync(CreateClubMemberRequestDto createClubMemberRequestDto)
        {
            try
            {
                var existingClub = await _clubRepository.GetClubByClubIdAsync(createClubMemberRequestDto.ClubId);
                if (existingClub == null)
                {
                    return ApiResponse<CreateClubMemberResponseDto>.Failure(404, "Club is not exist");
                }

                var existingUser = await _userRepository.GetUserByUserIdAsync(createClubMemberRequestDto.UserId);
                if (existingUser == null)
                {
                    return ApiResponse<CreateClubMemberResponseDto>.Failure(404, "User is not exist");
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

                var result = new CreateClubMemberResponseDto
                {
                    ClubMemberId = clubMember.ClubMemberId,
                    ClubId = existingClub.ClubId,
                    UserId = existingUser.UserId,
                    MemberRole = clubMember.MemberRole,
                };

                return ApiResponse<CreateClubMemberResponseDto>.Success(result, "Thêm thành viên thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm thành viên vào câu lạc bộ. UserId: {UserId}, ClubId: {ClubId}, Thời gian: {Time}", createClubMemberRequestDto.UserId, createClubMemberRequestDto.ClubId, DateTime.UtcNow);
                return ApiResponse<CreateClubMemberResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<PagedResponse<CreateClubMemberResponseDto>> GetAllClubMemberAsync(ClubMemberFilter filter)
        {
            try
            {
                var clubMembers = await _clubmemberRepository.GetAllClubMemberAsync();

                if (clubMembers == null || !clubMembers.Any())
                {
                    return new PagedResponse<CreateClubMemberResponseDto>
                    {
                        Items = new List<CreateClubMemberResponseDto>(),
                        PageNumber = 1,
                        PageSize = 10,
                        TotalPages = 0,
                        TotalCount = 0
                    };
                }

                var clubMemberDtos = clubMembers.Select(x => new CreateClubMemberResponseDto
                {
                    ClubMemberId = x.ClubMemberId,
                    ClubId = x.ClubId,
                    UserId = x.UserId,
                    MemberRole = x.MemberRole,
                    JoinAt = x.JoinedAt
                }).ToList();

                if (filter.ClubId.HasValue)
                {
                    clubMemberDtos = clubMemberDtos
                        .Where(x => x.ClubId == filter.ClubId.Value)
                        .ToList();
                }

                if (!string.IsNullOrWhiteSpace(filter.MemberRole))
                {
                    var role = filter.MemberRole.Trim().ToLower();

                    clubMemberDtos = clubMemberDtos
                        .Where(x => x.MemberRole.ToLower() == role)
                        .ToList();
                }


                var totalCount = clubMemberDtos.Count;

                var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
                var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

                var items = clubMemberDtos
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return new PagedResponse<CreateClubMemberResponseDto>
                {
                    Items = items,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thành viên câu lạc bộ. Thời gian: {Time}", DateTime.UtcNow);
                throw;
            }
        }

        public async Task<ApiResponse<CreateClubMemberResponseDto>> GetClubMemberByIdAsync(int id)
        {
            try
            {
                var clubMember = await _clubmemberRepository.GetClubMemberByIdAsync(id);
                if (clubMember == null)
                {
                    return ApiResponse<CreateClubMemberResponseDto>.Failure(404, "Không có thành viên câu lạc bộ này.");
                }

                var clubMemberDto = await _clubMemberMapping.ToResponse(clubMember);
                return ApiResponse<CreateClubMemberResponseDto>.Success(clubMemberDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thành viên câu lạc bộ. Thời gian: {Time}", DateTime.UtcNow);
                return ApiResponse<CreateClubMemberResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<CreateClubMemberResponseDto>> UpdateClubMemberAsync(int id, CreateClubMemberRequestDto updateClubMemberRequestDto)
        {
            try
            {
                var clubMember = await _clubmemberRepository.GetClubMemberByIdAsync(id);
                if (clubMember == null)
                {
                    return ApiResponse<CreateClubMemberResponseDto>.Failure(404, "Không có thành viên câu lạc bộ này.");
                }

                clubMember.ClubId = updateClubMemberRequestDto.ClubId;
                clubMember.UserId = updateClubMemberRequestDto.UserId;
                clubMember.JoinedAt = updateClubMemberRequestDto.JoinAt;
                clubMember.MemberRole = updateClubMemberRequestDto.MemberRole;
                clubMember.UpdatedAt = DateTime.Now;

                await _clubmemberRepository.UpdateClubMemberAsync(clubMember);
                await _clubmemberRepository.SaveChangeAsync();

                var clubMemberDto = await _clubMemberMapping.ToResponse(clubMember);
                return ApiResponse<CreateClubMemberResponseDto>.Success(clubMemberDto, "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thành viên câu lạc bộ. ClubMemberId: {ClubMemberId}, Thời gian: {Time}", id, DateTime.UtcNow);
                return ApiResponse<CreateClubMemberResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<CreateClubMemberResponseDto>>> GetAllClubMemberByClubIdAsync(int clubId)
        {
            try
            {
                var allMembers = await _clubmemberRepository.GetAllClubMemberAsync();
                var clubMembers = allMembers.Where(cm => cm.ClubId == clubId).ToList();

                if (clubMembers == null || !clubMembers.Any())
                {
                    return ApiResponse<List<CreateClubMemberResponseDto>>.Failure(404, "Không có thành viên câu lạc bộ nào.");
                }

                var clubMemberDtos = await _clubMemberMapping.ToDtoList(clubMembers);
                return ApiResponse<List<CreateClubMemberResponseDto>>.Success(clubMemberDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thành viên câu lạc bộ theo ClubId. ClubId: {ClubId}, Thời gian: {Time}", clubId, DateTime.UtcNow);
                return ApiResponse<List<CreateClubMemberResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var exist = await _clubmemberRepository.GetClubMemberByIdAsync(id);
                if (exist == null)
                {
                    return ApiResponse.Failure(400, "không tồn tại thành viên");
                }

                await _clubmemberRepository.Delete(id);
                await _clubmemberRepository.SaveChangeAsync();
                return ApiResponse.Success("Xóa thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "xóa thất bại");
                return ApiResponse.Failure(500, ex.Message);
            }
        }
    }
}