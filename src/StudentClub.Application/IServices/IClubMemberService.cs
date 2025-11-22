using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.IServices
{
    public interface IClubMemberService
    {
        Task<CreateClubMemberResponseDto> CreateClubMemberAsync(CreateClubMemberRequestDto createClubMemberRequestDto);
        Task<List<CreateClubMemberResponseDto>> GetAllClubMemberAsync();
        Task<List<CreateClubMemberResponseDto>> GetAllClubMemberByClubIdAsync(int clubId);
        Task<CreateClubMemberResponseDto> GetClubMemberByIdAsync(int id);
        Task<CreateClubMemberResponseDto> UpdateClubMemberAsync(int id, CreateClubMemberRequestDto updateClubMemberRequestDto);

    }
}
