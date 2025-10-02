using StudentClub.Application.DTOs;

namespace StudentClub.Application.IServices
{
    public interface IInterviewService
    {
        Task<GetInterviewResponseDto> CreateAsync(CreateInterviewRequestDto request, int userId, string role);

        Task<GetInterviewResponseDto> CreateWebAsync(CreateInterviewRequestDto request);
        Task<GetInterviewResponseDto?> UpdateAsync(int id, UpdateInterviewRequestDto request, int userId, string role);
        Task DeleteAsync(int id, int userId, string role);
        Task<List<GetInterviewResponseDto>> GetByClubIdAsync(int clubId, int userId, string role);
        Task<List<GetInterviewResponseDto>> GetAllAsync(); 
    }
}
