using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.IServices
{
    public interface IClubMemberService
    {
        Task<ApiResponse<CreateClubMemberResponseDto>> CreateClubMemberAsync(CreateClubMemberRequestDto createClubMemberRequestDto);
        Task<ApiResponse<List<CreateClubMemberResponseDto>>> GetAllClubMemberAsync();
        Task<ApiResponse<List<CreateClubMemberResponseDto>>> GetAllClubMemberByClubIdAsync(int clubId);
        Task<ApiResponse<CreateClubMemberResponseDto>> GetClubMemberByIdAsync(int id);
        Task<ApiResponse<CreateClubMemberResponseDto>> UpdateClubMemberAsync(int id, CreateClubMemberRequestDto updateClubMemberRequestDto);

    }
}
