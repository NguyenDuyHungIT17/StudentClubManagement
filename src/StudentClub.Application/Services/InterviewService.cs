using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<GetInterviewResponseDto> CreateAsync(CreateInterviewRequestDto request, int userId, string role)
        {
            try
            {
                var interviewCheck = await _repo.GetByClubIdAndEmail(request.ClubId, request.ApplicantEmail);
                if (interviewCheck != null)
                {
                    throw new KeyNotFoundException("Email bạn đã đăng được đăng kí rồi");
                }

                if (role == "leader")
                {
                    var club = await _clubRepo.GetClubByClubIdAsync(request.ClubId);
                    if (club == null)
                        throw new KeyNotFoundException("Club not found");

                    if (club.LeaderId != userId)
                        throw new UnauthorizedAccessException("Bạn không có quyền tạo lịch phỏng vấn cho CLB này");
                }

                var interview = new Interview
                {
                    ClubId = request.ClubId,
                    ApplicantName = request.ApplicantName,
                    ApplicantEmail = request.ApplicantEmail,
                    Evaluation = string.IsNullOrWhiteSpace(request.Evaluation)
                        ? "0"
                        : "0" + request.Evaluation,
                    Result = "Pending"
                };

                await _repo.AddAsync(interview);
                await _repo.SaveChangesAsync();

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo lịch phỏng vấn. Tên ứng viên: {ApplicantName}, Thời gian: {Time}", request.ApplicantName, DateTime.UtcNow);
                throw;
            }
        }
        public async Task<GetInterviewResponseDto> CreateWebAsync(CreateInterviewRequestDto request)
        {
            try
            {
                var interviewCheck = await _repo.GetByClubIdAndEmail(request.ClubId, request.ApplicantEmail);
                if (interviewCheck != null)
                {
                    throw new KeyNotFoundException("Email bạn đã đăng được đăng kí rồi");
                }

                var interview = new Interview
                {
                    ClubId = request.ClubId,
                    ApplicantName = request.ApplicantName,
                    ApplicantEmail = request.ApplicantEmail,
                    Evaluation = string.IsNullOrWhiteSpace(request.Evaluation)
                        ? "web"
                        : "web" + request.Evaluation,
                    Result = "Pending"
                };

                await _repo.AddAsync(interview);
                await _repo.SaveChangesAsync();

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo lịch phỏng vấn. Tên ứng viên: {ApplicantName}, Thời gian: {Time}", request.ApplicantName, DateTime.UtcNow);
                throw;
            }
        }
        public async Task<GetInterviewResponseDto?> UpdateAsync(int id, UpdateInterviewRequestDto request, int userId, string role)
        {
            try
            {
                var interview = await _repo.GetByIdAsync(id);
                if (interview == null) throw new KeyNotFoundException("Interview not found");
                if (role == "leader")
                {
                    var club = await _clubRepo.GetClubByClubIdAsync(interview.ClubId);
                    if (club == null)
                        throw new KeyNotFoundException("Club not found");

                    if (club.LeaderId != userId)
                        throw new UnauthorizedAccessException("Bạn không có quyền cập nhật lịch phỏng vấn của CLB này");
                }

                interview.Evaluation = request.Evaluation;
                interview.Result = request.Result;
                interview.UpdatedAt = DateTime.Now;

                await _repo.SaveChangesAsync();

                return new GetInterviewResponseDto
                {
                    InterviewId = interview.InterviewId,
                    ClubId = interview.ClubId,
                    ApplicantName = interview.ApplicantName,
                    ApplicantEmail = interview.ApplicantEmail,
                    Evaluation = interview.Evaluation,
                    Result = interview.Result,
                    CreatedAt = interview.CreatedAt,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật lịch phỏng vấn. InterviewId: {InterviewId}, Thời gian: {Time}", id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<List<GetInterviewResponseDto>> GetByClubIdAsync(int clubId, int userId, string role)
        {
            try
            {
                if (role == "leader")
                {
                    var club = await _clubRepo.GetClubByClubIdAsync(clubId);
                    if (club == null)
                        throw new KeyNotFoundException("Club not found");

                    if (club.LeaderId != userId)
                        throw new UnauthorizedAccessException("Bạn không có quyền xem phỏng vấn của CLB này");
                }

                var list = await _repo.GetByClubIdAsync(clubId);
                return list.Select(i => new GetInterviewResponseDto
                {
                    InterviewId = i.InterviewId,
                    ClubId = i.ClubId,
                    ApplicantName = i.ApplicantName,
                    ApplicantEmail = i.ApplicantEmail,
                    Evaluation = i.Evaluation,
                    Result = i.Result,
                    CreatedAt = i.CreatedAt,
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch phỏng vấn theo ClubId. ClubId: {ClubId}, Thời gian: {Time}", clubId, DateTime.UtcNow);
                throw;
            }
        }

        public async Task DeleteAsync(int id, int userId, string role)
        {
            try
            {
                var interview = await _repo.GetByIdAsync(id);
                if (interview == null)
                    throw new KeyNotFoundException("Interview not found");

                if (role == "leader")
                {
                    var club = await _clubRepo.GetClubByClubIdAsync(interview.ClubId);
                    if (club == null)
                        throw new KeyNotFoundException("Club not found");

                    if (club.LeaderId != userId)
                        throw new UnauthorizedAccessException("Bạn không có quyền xóa phỏng vấn của CLB này");
                }

                await _repo.DeleteAsync(interview);
                await _repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa lịch phỏng vấn. InterviewId: {InterviewId}, Thời gian: {Time}", id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<List<GetInterviewResponseDto>> GetAllAsync()
        {
            try
            {
                var list = await _repo.GetAllAsync();

                return list.Select(i => new GetInterviewResponseDto
                {
                    InterviewId = i.InterviewId,
                    ClubId = i.ClubId,
                    ApplicantName = i.ApplicantName,
                    ApplicantEmail = i.ApplicantEmail,
                    Evaluation = i.Evaluation,
                    Result = i.Result,
                    CreatedAt = i.CreatedAt,
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tất cả lịch phỏng vấn. Thời gian: {Time}", DateTime.UtcNow);
                throw;
            }
        }
    }
}
