using StudentClub.Application.DTOs;
using StudentClub.Application.DTOs.Clubs;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.Services
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;

        public ClubService(IClubRepository clubRepository)
        {
            _clubRepository = clubRepository;
        }

        public async Task<CreateClubResponseDto> CreateClubAsync(CreateClubRequestDto createClubRequestDto)
        {
            var existingClub = await _clubRepository.GetClubByClubNameAsync(createClubRequestDto.ClubName);
            if (existingClub != null)
            {
                throw new Exception("Club already exist");
            }

            var club = new Club
            {
                ClubName = createClubRequestDto.ClubName,
                Description = createClubRequestDto.Description,
                LeaderId = null,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _clubRepository.AddClubAsync(club);
            await _clubRepository.SaveChangeAsync();

            return new CreateClubResponseDto
            {
                ClubName = club.ClubName,
                Description = club.Description,
                LeaderName = "Cập nhật sau"
            };
        }

        public async Task<UpdateClubResponseDto> UpdateClubAsync(UpdateClubRequestDto updateClubRequestDto)
        {
            var club = await _clubRepository.GetClubByClubIdAsync(updateClubRequestDto.Id);
            if (club == null)
            {
                throw new Exception("Câu lạc bộ không được tìm thấy");
            }

            club.ClubName = updateClubRequestDto.ClubName;
            club.Description = updateClubRequestDto.Description;
            club.LeaderId = updateClubRequestDto.LeaderId;

            await _clubRepository.UpdateClubAsync(club);
            await _clubRepository.SaveChangeAsync();

            return new UpdateClubResponseDto
            {
                ClubName = club.ClubName,
                Description = club.Description,
                LeaderId = updateClubRequestDto.LeaderId,
            };
        }

        public async Task DeleteClubAsync(int clubId)
        {
            var club = await _clubRepository.GetClubByClubIdAsync(clubId);
            if (club == null)
                throw new KeyNotFoundException("Club không tìm thấy");

            
            await _clubRepository.DeleteEventRegistrationsByClubIdAsync(clubId);
            await _clubRepository.DeleteFeedbacksByClubIdAsync(clubId);
            await _clubRepository.DeleteEventsByClubIdAsync(clubId);
            await _clubRepository.DeleteMembersByClubIdAsync(clubId);
            await _clubRepository.DeleteInterviewsByClubIdAsync(clubId);

            
            await _clubRepository.DeleteClubAsync(club);

            await _clubRepository.SaveChangeAsync();
        }
    }
}
