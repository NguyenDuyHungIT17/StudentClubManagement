using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Shared.ApiResponse; 

namespace StudentClub.Application.IServices
{
    public interface IClubService
    {
        Task<ApiResponse<CreateClubResponseDto>> CreateClubAsync(CreateClubRequestDto createClubRequestDto);

        Task<ApiResponse<UpdateClubResponseDto>> UpdateClubAsync(int id, UpdateClubRequestDto updateClubRequestDto);

        Task<ApiResponse<List<GetAllClubsResponseDto>>> GetAllClubAsync();

        Task<ApiResponse<GetClubResponseDto>> GetClubAsync(int clubId);

        Task<ApiResponse> DeleteClubAsync(int clubId); // Trả về ApiResponse (không có T) cho hàm void/delete
    }
}