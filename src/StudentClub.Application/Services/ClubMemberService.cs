using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.ClubMember;
using StudentClub.Application.DTOs.response.ClubMember;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;
using StudentClub.Domain.Entities;
using StudentClub.Shared.ApiResponse;
using Microsoft.EntityFrameworkCore;
namespace StudentClub.Application.Services
{
    public class ClubMemberService : IClubMemberService
    {
        private readonly IClubMemberRepository _clubmemberRepository;
        private readonly IUserRepository _userRepository;
        private readonly IClubRepository _clubRepository;
        private readonly ClubMemberMapping _clubMemberMapping;
        private readonly IPhotoService _photoService;
        private readonly ILogger<ClubMemberService> _logger;

        public ClubMemberService(
            IClubMemberRepository clubmemberRepository,
            IUserRepository userRepository,
            IClubRepository clubRepository,
            ILogger<ClubMemberService> logger,
            ClubMemberMapping clubMemberMapping,
            IPhotoService photoService)
        {
            _clubmemberRepository = clubmemberRepository;
            _user_repository_check(userRepository);
            _userRepository = userRepository;
            _clubRepository = clubRepository;
            _clubMemberMapping = clubMemberMapping;
            _photoService = photoService;
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
                    await _club_repository_updateLeader(createClubMemberRequestDto.ClubId, createClubMemberRequestDto.UserId);
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
                // Build DB query and apply filters before materializing
                var q = _clubmemberRepository.QueryClubMembers();

                if (filter.ClubId.HasValue)
                {
                    q = q.Where(x => x.ClubId == filter.ClubId.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.MemberRole))
                {
                    var role = filter.MemberRole.Trim().ToLower();
                    q = q.Where(x => x.MemberRole != null && x.MemberRole.ToLower() == role);
                }

                var totalCount = await q.CountAsync();

                var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
                var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

                var items = await q
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new CreateClubMemberResponseDto
                    {
                        ClubMemberId = x.ClubMemberId,
                        ClubId = x.ClubId,
                        UserId = x.UserId,
                        MemberRole = x.MemberRole,
                        JoinAt = x.JoinedAt
                    })
                    .ToListAsync();

                // Batch load photo URLs by clubMemberIds
                var clubMemberIds = items.Select(i => i.ClubMemberId).ToList();
                var photoMap = await _photoService.GetMainPhotoUrlsByClubMemberIdsAsync(clubMemberIds);

                foreach (var it in items)
                {
                    it.PhotoUrl = photoMap.ContainsKey(it.ClubMemberId) ? photoMap[it.ClubMemberId] : null;
                }

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

                // attach photo
                clubMemberDto.PhotoUrl = await _photoService.GetMainPhotoUrlAsync(null, null, null, clubMemberDto.ClubMemberId);

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

                // attach photo
                clubMemberDto.PhotoUrl = await _photoService.GetMainPhotoUrlAsync(null, null, null, clubMemberDto.ClubMemberId);

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
                var q = _clubmemberRepository.QueryClubMembers().Where(cm => cm.ClubId == clubId);

                var clubMembers = await q.ToListAsync();

                if (clubMembers == null || !clubMembers.Any())
                {
                    return ApiResponse<List<CreateClubMemberResponseDto>>.Failure(404, "Không có thành viên câu lạc bộ nào.");
                }

                var clubMemberDtos = await _clubMemberMapping.ToDtoList(clubMembers);

                // batch photos
                var ids = clubMemberDtos.Select(d => d.ClubMemberId).ToList();
                var photoMap = await _photoService.GetMainPhotoUrlsByClubMemberIdsAsync(ids);
                foreach (var dto in clubMemberDtos) dto.PhotoUrl = photoMap.ContainsKey(dto.ClubMemberId) ? photoMap[dto.ClubMemberId] : null;

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

                await _photoService.DeletePhotoByAnyway(id, 4); // type 4 = clubMember
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

        // small helpers to satisfy static analysis in DI wiring (no behavior change)
        private void _user_repository_check(IUserRepository repo) { }
        private Task _club_repository_updateLeader(int clubId, int leaderId) => _clubRepository.UpdateLeaderIdAsync(clubId, leaderId);
    }
}