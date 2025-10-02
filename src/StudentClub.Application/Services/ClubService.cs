using StudentClub.Application.DTOs;
using StudentClub.Application.DTOs.Clubs;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.Services
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IUserRepository _userRepository;
        public ClubService(IClubRepository clubRepository, IUserRepository userRepository)
        {
            _clubRepository = clubRepository;
            _userRepository = userRepository;
        }

        public async Task<CreateClubResponseDto> CreateClubAsync(CreateClubRequestDto createClubRequestDto)
        {
            var existingClub = await _clubRepository.GetClubByClubNameAsync(createClubRequestDto.ClubName);
            if (existingClub != null)
            {
                return new CreateClubResponseDto
                {
                    ClubName = existingClub.ClubName,
                    Description = existingClub.Description,
                    LeaderName = "Đã tồn tại",
                };

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

        public async Task<List<GetAllClubsResponseDto>> GetAllClubAsync()
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

            return clubsDto;
        }

        public async Task<GetClubResponseDto> GetClubAsync(int clubId)
        {
            var club = await _clubRepository.GetClubAsync(clubId);
            var users = await _userRepository.GetAllUsersAsync();
            if (club == null)
            {
                return null;
            }

            var clubDto = new GetClubResponseDto();

            var description = "chưa mô tả";
            if (club.Description != null)
            {
                description = club.Description;
            }
            clubDto.ClubId = clubId;
            clubDto.ClubName= club.ClubName;
            clubDto.Description = description;
            clubDto.CreatedAt = club.CreatedAt;
            clubDto.LeaderName = club.LeaderId == null ? "Cập nhật sau" : users.FirstOrDefault(u => u.UserId == club.LeaderId)?.FullName ?? "Cập nhật sau";

            return clubDto;
        }
    }
}
