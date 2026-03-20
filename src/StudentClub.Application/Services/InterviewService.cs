using Microsoft.Extensions.Logging;
using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.Request.Interview;
using StudentClub.Application.DTOs.Response.Interview;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.Mapper;
using StudentClub.Domain.Entities;
using StudentClub.Domain.Enums;
using StudentClub.Shared.ApiResponse;

namespace StudentClub.Application.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _repo;
        private readonly IClubRepository _clubRepo;
        private readonly ILogger<InterviewService> _logger;
        private readonly IUserContext _userContext;
        private readonly InterviewMapping _mapping;

        public InterviewService(
            IInterviewRepository repo,
            IClubRepository clubRepo,
            IUserContext userContext,
            InterviewMapping mapping,
            ILogger<InterviewService> logger)
        {
            _repo = repo;
            _clubRepo = clubRepo;
            _userContext = userContext;
            _mapping = mapping;
            _logger = logger;
        }

        // Tạo interview (leader/member)  - walkin -- 
        public async Task<ApiResponse<InterviewResponseDto>> CreateAsync(CreateInterviewRequestDto request)
        {
            try
            {
                var club = await _clubRepo.GetClubByClubIdAsync(request.ClubId);
                if (club == null)
                    return ApiResponse<InterviewResponseDto>.Failure(404, "CLB không tồn tại");

                var entity = _mapping.ToEntity(request, InterviewStatus.Registered, ApplicationType.WalkIn);

                await _repo.AddAsync(entity);
                await _repo.SaveChangesAsync();

                return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "Tạo thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi create interview {Time}", DateTime.UtcNow);
                return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        // Tạo từ web (public) -- cho tất cả mọi người
        public async Task<ApiResponse<InterviewResponseDto>> CreateWebAsync(CreateInterviewRequestDto request)
        {
            try
            {
                var club = await _clubRepo.GetClubByClubIdAsync(request.ClubId);
                if (club == null)
                    return ApiResponse<InterviewResponseDto>.Failure(404, "CLB không tồn tại");

                var entity = _mapping.ToEntity(request, InterviewStatus.Registered, ApplicationType.Online);

                await _repo.AddAsync(entity);
                await _repo.SaveChangesAsync();

                return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "Đăng ký thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi create web interview {Time}", DateTime.UtcNow);
                return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        //// Cập nhật interview
        public async Task<ApiResponse<InterviewResponseDto>> UpdateAsync(int id, UpdateInterviewRequestDto request)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

                _mapping.UpdateEntity(entity, request);

                await _repo.UpdateAsync(entity);
                await _repo.SaveChangesAsync();

                return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi update interview {Time}", DateTime.UtcNow);
                return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        //// Xóa interview
        //public async Task<ApiResponse> DeleteAsync(int id)
        //{
        //    try
        //    {
        //        var entity = await _repo.GetByIdAsync(id);
        //        if (entity == null)
        //            return ApiResponse.Failure(404, "Không tìm thấy");

        //        await _repo.DeleteAsync(entity);
        //        await _repo.SaveChangeAsync();

        //        return ApiResponse.Success("Xóa thành công");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi delete interview {Time}", DateTime.UtcNow);
        //        return ApiResponse.Failure(500, ex.Message);
        //    }
        //}

        //// Lấy chi tiết interview
        //public async Task<ApiResponse<InterviewResponseDto>> GetByIdAsync(int id)
        //{
        //    try
        //    {
        //        var entity = await _repo.GetByIdAsync(id);
        //        if (entity == null)
        //            return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

        //        return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi getById interview {Time}", DateTime.UtcNow);
        //        return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
        //    }
        //}

        //// Lấy danh sách + phân trang
        //public async Task<PagedResponse<InterviewResponseDto>> GetAllInterviewsAsync(InterviewFilter filter)
        //{
        //    try
        //    {
        //        var list = await _repo.GetAllAsync();

        //        if (!string.IsNullOrWhiteSpace(filter.KeyWord))
        //        {
        //            var keyword = filter.KeyWord.ToLower();
        //            list = list.Where(x => x.ApplicantName.ToLower().Contains(keyword)).ToList();
        //        }

        //        var total = list.Count;

        //        var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
        //        var size = filter.PageSize <= 0 ? 10 : filter.PageSize;

        //        var items = list.Skip((page - 1) * size).Take(size).ToList();

        //        return new PagedResponse<InterviewResponseDto>
        //        {
        //            Items = _mapping.ToListResponse(items),
        //            PageNumber = page,
        //            PageSize = size,
        //            TotalCount = total,
        //            TotalPages = (int)Math.Ceiling(total / (double)size)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi get list interview {Time}", DateTime.UtcNow);
        //        throw;
        //    }
        //}

        //// Check-in ứng viên
        //public async Task<ApiResponse<InterviewResponseDto>> CheckInAsync(int id)
        //{
        //    try
        //    {
        //        var entity = await _repo.GetByIdAsync(id);
        //        if (entity == null)
        //            return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

        //        _mapping.MapCheckIn(entity);

        //        await _repo.SaveChangeAsync();

        //        return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "Check-in thành công");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi checkin {Time}", DateTime.UtcNow);
        //        return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
        //    }
        //}

        //// Bắt đầu phỏng vấn
        //public async Task<ApiResponse<InterviewResponseDto>> StartAsync(int id, StartInterviewRequestDto request)
        //{
        //    try
        //    {
        //        var entity = await _repo.GetByIdAsync(id);
        //        if (entity == null)
        //            return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

        //        _mapping.MapStart(entity, request);

        //        await _repo.SaveChangeAsync();

        //        return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "Bắt đầu phỏng vấn");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi start interview {Time}", DateTime.UtcNow);
        //        return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
        //    }
        //}

        //// Kết thúc phỏng vấn
        //public async Task<ApiResponse<InterviewResponseDto>> FinishAsync(int id, FinishInterviewRequestDto request)
        //{
        //    try
        //    {
        //        var entity = await _repo.GetByIdAsync(id);
        //        if (entity == null)
        //            return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

        //        _mapping.MapFinish(entity, request);

        //        await _repo.SaveChangeAsync();

        //        return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "Hoàn thành");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi finish interview {Time}", DateTime.UtcNow);
        //        return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
        //    }
        //}

        //// Đánh dấu no-show
        //public async Task<ApiResponse<InterviewResponseDto>> NoShowAsync(int id)
        //{
        //    try
        //    {
        //        var entity = await _repo.GetByIdAsync(id);
        //        if (entity == null)
        //            return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

        //        _mapping.MapNoShow(entity, "Không đến");

        //        await _repo.SaveChangeAsync();

        //        return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "No-show");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi noshow {Time}", DateTime.UtcNow);
        //        return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
        //    }
        //}

        //// Hủy interview
        //public async Task<ApiResponse<InterviewResponseDto>> CancelAsync(int id)
        //{
        //    try
        //    {
        //        var entity = await _repo.GetByIdAsync(id);
        //        if (entity == null)
        //            return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

        //        _mapping.MapCancel(entity);

        //        await _repo.SaveChangeAsync();

        //        return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "Đã hủy");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi cancel interview {Time}", DateTime.UtcNow);
        //        return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
        //    }
        //}
    }
}