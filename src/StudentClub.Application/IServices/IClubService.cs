using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.Club;
using StudentClub.Application.DTOs.response.Club;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.IServices
{
    public interface IClubService
    {
        Task<ApiResponse<CreateClubResponseDto>> CreateClubAsync(CreateClubRequestDto createClubRequestDto);

        Task<ApiResponse<UpdateClubResponseDto>> UpdateClubAsync(int id, UpdateClubRequestDto updateClubRequestDto);

        Task<PagedResponse<GetAllClubsResponseDto>> GetAllClubAsync(ClubFilterRequest filter);

        Task<ApiResponse<GetClubResponseDto>> GetClubAsync(int clubId);

        Task<ApiResponse> DeleteClubAsync(int clubId); // Trả về ApiResponse (không có T) cho hàm void/delete
    }
}