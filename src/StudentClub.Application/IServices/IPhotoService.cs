using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Shared.ApiResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentClub.Application.IServices
{
    public interface IPhotoService
    {
        Task<ApiResponse<PhotoResponseDto>> UploadPhotoAsync(UploadPhotoRequestDto request);
        Task<ApiResponse> DeletePhotoAsync(int photoId);
        Task<ApiResponse<List<PhotoResponseDto>>> GetPhotosByEventIdAsync(int eventId);
        Task<ApiResponse<List<PhotoResponseDto>>> GetPhotosByClubIdAsync(int clubId);
        Task<ApiResponse<List<PhotoResponseDto>>> GetPhotosByUserIdAsync(int userId);

        Task<ApiResponse<PhotoResponseDto>> UpdatePhotoAsync(int photoId, UpdatePhotoRequestDto request, int userId);
        Task<ApiResponse<List<PhotoResponseDto>>> GetPhotosByClubMemberIdAsync(int clubMemberId);

    }
}