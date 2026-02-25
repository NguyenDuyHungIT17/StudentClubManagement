using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Domain.Entities;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.Services
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ClubService> _logger;

        public ClubService(IClubRepository clubRepository, IUserRepository userRepository, ILogger<ClubService> logger)
        {
            _clubRepository = clubRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<CreateClubResponseDto>> CreateClubAsync(CreateClubRequestDto createClubRequestDto)
        {
            try
            {
                var existingClub = await _clubRepository.GetClubByClubNameAsync(createClubRequestDto.ClubName);
                if (existingClub != null)
                {
                    var existDto = new CreateClubResponseDto
                    {
                        ClubName = existingClub.ClubName,
                        Description = existingClub.Description,
                        LeaderName = "Đã tồn tại",
                    };
                    return ApiResponse<CreateClubResponseDto>.Success(existDto, "Câu lạc bộ đã tồn tại");
                }

                var club = new Club
                {
                    ClubName = createClubRequestDto.ClubName,
                    Description = createClubRequestDto.Description,
                    LeaderId = null,
                    Title = createClubRequestDto.Title,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                await _clubRepository.AddClubAsync(club);
                await _clubRepository.SaveChangeAsync();

                var result = new CreateClubResponseDto
                {
                    ClubName = club.ClubName,
                    Description = club.Description,
                    LeaderName = "Cập nhật sau",
                    Title = club.Title,
                };

                return ApiResponse<CreateClubResponseDto>.Success(result, "Tạo câu lạc bộ thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo câu lạc bộ. Tên câu lạc bộ: {ClubName}, Thời gian: {Time}", createClubRequestDto.ClubName, DateTime.UtcNow);
                return ApiResponse<CreateClubResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<UpdateClubResponseDto>> UpdateClubAsync(int id, UpdateClubRequestDto updateClubRequestDto)
        {
            try
            {
                var club = await _clubRepository.GetClubByClubIdAsync(id);
                if (club == null)
                {
                    return ApiResponse<UpdateClubResponseDto>.Failure(404, "Câu lạc bộ không được tìm thấy");
                }

                club.ClubName = updateClubRequestDto.ClubName;
                club.Description = updateClubRequestDto.Description;
                club.LeaderId = updateClubRequestDto.LeaderId;
                club.UpdatedAt = DateTime.Now;

                await _clubRepository.UpdateClubAsync(club);
                await _clubRepository.SaveChangeAsync();

                var result = new UpdateClubResponseDto
                {
                    ClubName = club.ClubName,
                    Description = club.Description,
                    LeaderId = updateClubRequestDto.LeaderId,
                };

                return ApiResponse<UpdateClubResponseDto>.Success(result, "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật câu lạc bộ. ClubId: {ClubId}, Thời gian: {Time}", id, DateTime.UtcNow);
                return ApiResponse<UpdateClubResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteClubAsync(int clubId)
        {
            try
            {
                var club = await _clubRepository.GetClubByClubIdAsync(clubId);
                if (club == null)
                {
                    return ApiResponse.Failure(404, "Club không tìm thấy");
                }

                await _clubRepository.DeleteEventRegistrationsByClubIdAsync(clubId);
                await _clubRepository.DeleteFeedbacksByClubIdAsync(clubId);
                await _clubRepository.DeleteEventsByClubIdAsync(clubId);
                await _clubRepository.DeleteMembersByClubIdAsync(clubId);
                await _clubRepository.DeleteInterviewsByClubIdAsync(clubId);

                await _clubRepository.DeleteClubAsync(club);
                await _clubRepository.SaveChangeAsync();

                return ApiResponse.Success("Xóa câu lạc bộ thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa câu lạc bộ. ClubId: {ClubId}, Thời gian: {Time}", clubId, DateTime.UtcNow);
                return ApiResponse.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<GetAllClubsResponseDto>>> GetAllClubAsync()
        {
            try
            {
                var clubs = await _clubRepository.GetClubsAsync();
                var users = await _userRepository.GetAllUsersAsync();

                var clubsDto = clubs.Select(x => new GetAllClubsResponseDto
                {
                    ClubId = x.ClubId,
                    ClubName = x.ClubName,
                    LeaderName = x.LeaderId == null
                        ? "Cập nhật sau"
                        : users.FirstOrDefault(u => u.UserId == x.LeaderId)?.FullName ?? "Cập nhật sau",
                    Description = x.Description,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).ToList();

                return ApiResponse<List<GetAllClubsResponseDto>>.Success(clubsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách câu lạc bộ. Thời gian: {Time}", DateTime.UtcNow);
                return ApiResponse<List<GetAllClubsResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<GetClubResponseDto>> GetClubAsync(int clubId)
        {
            try
            {
                var club = await _clubRepository.GetClubAsync(clubId);
                var users = await _userRepository.GetAllUsersAsync();

                if (club == null)
                {
                    return ApiResponse<GetClubResponseDto>.Failure(404, "Không tìm thấy câu lạc bộ");
                }

                var description = club.Description ?? "chưa mô tả";

                var clubDto = new GetClubResponseDto
                {
                    ClubId = clubId,
                    ClubName = club.ClubName,
                    Description = description,
                    CreatedAt = club.CreatedAt,
                    LeaderName = club.LeaderId == null
                        ? "Cập nhật sau"
                        : users.FirstOrDefault(u => u.UserId == club.LeaderId)?.FullName ?? "Cập nhật sau"
                };

                return ApiResponse<GetClubResponseDto>.Success(clubDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin câu lạc bộ. ClubId: {ClubId}, Thời gian: {Time}", clubId, DateTime.UtcNow);
                return ApiResponse<GetClubResponseDto>.Failure(500, ex.Message);
            }
        }
    }
}