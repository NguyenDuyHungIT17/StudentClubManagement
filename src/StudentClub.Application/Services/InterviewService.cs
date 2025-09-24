using StudentClub.Application.DTOs;
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

        public InterviewService(IInterviewRepository repo, IClubRepository clubRepo)
        {
            _repo = repo;
            _clubRepo = clubRepo;
        }

        public async Task<GetInterviewResponseDto> CreateAsync(CreateInterviewRequestDto request, int userId, string role)
        {

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
                Result = interview.Result,
                CreatedAt = interview.CreatedAt
            };
        }

        public async Task<GetInterviewResponseDto?> UpdateAsync(int id, UpdateInterviewRequestDto request, int userId, string role)
        {
            var interview = await _repo.GetByIdAsync(id);
            if (interview == null) throw new KeyNotFoundException("Interview not found");

            // Nếu leader thì chỉ được update interview trong CLB của mình
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

        public async Task<List<GetInterviewResponseDto>> GetByClubIdAsync(int clubId, int userId, string role)
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

        public async Task DeleteAsync(int id, int userId, string role)
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

        public async Task<List<GetInterviewResponseDto>> GetAllAsync()
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

    }
}
