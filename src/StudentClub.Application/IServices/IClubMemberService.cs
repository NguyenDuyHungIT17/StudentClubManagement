using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.ClubMember;
using StudentClub.Application.DTOs.response.ClubMember;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.IServices
{
    public interface IClubMemberService
    {
        Task<ApiResponse<CreateClubMemberResponseDto>> CreateClubMemberAsync(CreateClubMemberRequestDto createClubMemberRequestDto);
        Task<PagedResponse<CreateClubMemberResponseDto>> GetAllClubMemberAsync(ClubMemberFilter filter);
        Task<ApiResponse<List<CreateClubMemberResponseDto>>> GetAllClubMemberByClubIdAsync(int clubId);
        Task<ApiResponse<CreateClubMemberResponseDto>> GetClubMemberByIdAsync(int id);
        Task<ApiResponse<CreateClubMemberResponseDto>> UpdateClubMemberAsync(int id, CreateClubMemberRequestDto updateClubMemberRequestDto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
