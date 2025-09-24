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
    public class ClubMemberService : IClubMemberService
    {
        private IClubMemberRepository _clubmemberRepository;
        private IUserRepository _userRepository;
        private IClubRepository _clubRepository;

        public ClubMemberService(IClubMemberRepository clubmemberRepository, IUserRepository userRepository, IClubRepository clubRepository)
        {
            _clubmemberRepository = clubmemberRepository;
            _userRepository = userRepository;
            _clubRepository = clubRepository;
        }

        public async Task<CreateClubMemberResponseDto> CreateClubMemberAsync(CreateClubMemberRequestDto createClubMemberRequestDto)
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


    }
}
