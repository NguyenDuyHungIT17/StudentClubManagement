using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.Request.Interview;
using StudentClub.Application.DTOs.Response.Interview;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.IServices
{
    public interface IInterviewService
    {
        // Tạo ứng viên (leader/member - walk-in hoặc quản lý)
        Task<ApiResponse<InterviewResponseDto>> CreateAsync(CreateInterviewRequestDto request);

        //Tạo ứng viên từ web(public đăng ký)
        Task<ApiResponse<InterviewResponseDto>> CreateWebAsync(CreateInterviewRequestDto request);

        //// Cập nhật thông tin ứng viên
        Task<ApiResponse<InterviewResponseDto>> UpdateAsync(int id, UpdateInterviewRequestDto request);

        //// Xóa ứng viên phỏng vấn
        //Task<ApiResponse> DeleteAsync(int id);

        //// Lấy chi tiết 1 ứng viên
        //Task<ApiResponse<InterviewResponseDto>> GetByIdAsync(int id);

        //// Lấy danh sách interview có phân trang + filter
        //Task<PagedResponse<InterviewResponseDto>> GetAllInterviewsAsync(InterviewFilter filter);

        //// Check-in ứng viên khi đến
        //Task<ApiResponse<InterviewResponseDto>> CheckInAsync(int id);

        //// Bắt đầu phỏng vấn (gán người phỏng vấn)
        //Task<ApiResponse<InterviewResponseDto>> StartAsync(int id, StartInterviewRequestDto request);

        //// Kết thúc phỏng vấn và chấm kết quả
        //Task<ApiResponse<InterviewResponseDto>> FinishAsync(int id, FinishInterviewRequestDto request);

        //// Đánh dấu ứng viên không đến
        //Task<ApiResponse<InterviewResponseDto>> NoShowAsync(int id);

        //// Hủy phỏng vấn
        //Task<ApiResponse<InterviewResponseDto>> CancelAsync(int id);
    }
}