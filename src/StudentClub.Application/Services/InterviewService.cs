using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Domain.Entities;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _repo;
        private readonly IClubRepository _clubRepo;
        private readonly ILogger<InterviewService> _logger;

        public InterviewService(IInterviewRepository repo, IClubRepository clubRepo, IUserRepository userRepo, ILogger<InterviewService> logger)
        {
            _repo = repo;
            _clubRepo = clubRepo;
            _logger = logger;
        }

        public async Task<ApiResponse<GetInterviewResponseDto>> CreateAsync(CreateInterviewRequestDto request, int userId, string role)
        {
            try
            {
                var interviewCheck = await _repo.GetByClubIdAndEmail(request.ClubId, request.ApplicantEmail);
                if (interviewCheck != null)
                {
                    return ApiResponse<GetInterviewResponseDto>.Failure(400, "Email bạn đã đăng được đăng kí rồi");
                }

                if (role == "leader")
                {
                    var club = await _clubRepo.GetClubByClubIdAsync(request.ClubId);
                    if (club == null)
                        return ApiResponse<GetInterviewResponseDto>.Failure(404, "Club not found");

                    if (club.LeaderId != userId)
                        return ApiResponse<GetInterviewResponseDto>.Failure(403, "Bạn không có quyền tạo lịch phỏng vấn cho CLB này");
                }

                var interview = new Interview
                {
                    ClubId = request.ClubId,
                    ApplicantName = request.ApplicantName,
                    ApplicantEmail = request.ApplicantEmail,
                    Evaluation = string.IsNullOrWhiteSpace(request.Evaluation) ? "0" : "0" + request.Evaluation,
                    Result = "Pending"
                };

                await _repo.AddAsync(interview);
                await _repo.SaveChangesAsync();

                var response = MapToDto(interview);
                return ApiResponse<GetInterviewResponseDto>.Success(response, "Tạo lịch phỏng vấn thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo lịch phỏng vấn. Tên ứng viên: {ApplicantName}", request.ApplicantName);
                return ApiResponse<GetInterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<GetInterviewResponseDto>> CreateWebAsync(CreateInterviewRequestDto request)
        {
            try
            {
                var interviewCheck = await _repo.GetByClubIdAndEmail(request.ClubId, request.ApplicantEmail);
                if (interviewCheck != null)
                {
                    return ApiResponse<GetInterviewResponseDto>.Failure(400, "Email bạn đã đăng được đăng kí rồi");
                }

                var interview = new Interview
                {
                    ClubId = request.ClubId,
                    ApplicantName = request.ApplicantName,
                    ApplicantEmail = request.ApplicantEmail,
                    Evaluation = string.IsNullOrWhiteSpace(request.Evaluation) ? "web" : "web" + request.Evaluation,
                    Result = "Pending"
                };

                await _repo.AddAsync(interview);
                await _repo.SaveChangesAsync();

                return ApiResponse<GetInterviewResponseDto>.Success(MapToDto(interview), "Đăng ký phỏng vấn qua Web thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo lịch phỏng vấn Web.");
                return ApiResponse<GetInterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<GetInterviewResponseDto>> UpdateAsync(int id, UpdateInterviewRequestDto request, int userId, string role)
        {
            try
            {
                var interview = await _repo.GetByIdAsync(id);
                if (interview == null) return ApiResponse<GetInterviewResponseDto>.Failure(404, "Interview not found");

                if (role == "leader")
                {
                    var club = await _clubRepo.GetClubByClubIdAsync(interview.ClubId);
                    if (club == null) return ApiResponse<GetInterviewResponseDto>.Failure(404, "Club not found");

                    if (club.LeaderId != userId)
                        return ApiResponse<GetInterviewResponseDto>.Failure(403, "Bạn không có quyền cập nhật lịch phỏng vấn của CLB này");
                }

                interview.Evaluation = request.Evaluation;
                interview.Result = request.Result;
                interview.UpdatedAt = DateTime.Now;

                await _repo.SaveChangesAsync();
                return ApiResponse<GetInterviewResponseDto>.Success(MapToDto(interview), "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật lịch phỏng vấn ID: {InterviewId}", id);
                return ApiResponse<GetInterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<GetInterviewResponseDto>>> GetByClubIdAsync(int clubId, int userId, string role)
        {
            try
            {
                if (role == "leader")
                {
                    var club = await _clubRepo.GetClubByClubIdAsync(clubId);
                    if (club == null) return ApiResponse<List<GetInterviewResponseDto>>.Failure(404, "Club not found");

                    if (club.LeaderId != userId)
                        return ApiResponse<List<GetInterviewResponseDto>>.Failure(403, "Bạn không có quyền xem phỏng vấn của CLB này");
                }

                var list = await _repo.GetByClubIdAsync(clubId);
                var dtos = list.Select(MapToDto).ToList();
                return ApiResponse<List<GetInterviewResponseDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch phỏng vấn theo ClubId: {ClubId}", clubId);
                return ApiResponse<List<GetInterviewResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id, int userId, string role)
        {
            try
            {
                var interview = await _repo.GetByIdAsync(id);
                if (interview == null) return ApiResponse.Failure(404, "Interview not found");

                if (role == "leader")
                {
                    var club = await _clubRepo.GetClubByClubIdAsync(interview.ClubId);
                    if (club == null) return ApiResponse.Failure(404, "Club not found");

                    if (club.LeaderId != userId)
                        return ApiResponse.Failure(403, "Bạn không có quyền xóa phỏng vấn của CLB này");
                }

                await _repo.DeleteAsync(interview);
                await _repo.SaveChangesAsync();
                return ApiResponse.Success("Xóa lịch phỏng vấn thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa lịch phỏng vấn ID: {InterviewId}", id);
                return ApiResponse.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<List<GetInterviewResponseDto>>> GetAllAsync()
        {
            try
            {
                var list = await _repo.GetAllAsync();
                var dtos = list.Select(MapToDto).ToList();
                return ApiResponse<List<GetInterviewResponseDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tất cả lịch phỏng vấn");
                return ApiResponse<List<GetInterviewResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<GetInterviewResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var interview = await _repo.GetByIdAsync(id);
                if (interview == null) return ApiResponse<GetInterviewResponseDto>.Failure(404, "Không tìm thấy phỏng vấn");

                return ApiResponse<GetInterviewResponseDto>.Success(MapToDto(interview));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy phỏng vấn ID: {id}", id);
                return ApiResponse<GetInterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        // Helper method nội bộ để tránh lặp code mapping
        private GetInterviewResponseDto MapToDto(Interview interview)
        {
            return new GetInterviewResponseDto
            {
                InterviewId = interview.InterviewId,
                ClubId = interview.ClubId,
                ApplicantName = interview.ApplicantName,
                ApplicantEmail = interview.ApplicantEmail,
                Evaluation = interview.Evaluation,
                Result = interview.Result,
                CreatedAt = interview.CreatedAt
            };
        }
    }
}