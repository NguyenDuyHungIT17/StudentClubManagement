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

        // Tạo interview (leader/member) - walkin
        public async Task<ApiResponse<InterviewResponseDto>> CreateAsync(CreateInterviewRequestDto request)
        {
            try
            {
                var club = await _clubRepo.GetClubByClubIdAsync(request.ClubId);
                if (club == null)
                    return ApiResponse<InterviewResponseDto>.Failure(404, "CLB không tồn tại");

                var entity = _mapping.ToEntity(request, InterviewStatus.CheckedIn, ApplicationType.WalkIn);

                await _repo.AddAsync(entity);
                await _repo.SaveChangesAsync();

                _logger.LogInformation("Created walk-in interview {InterviewId} for club {ClubId}", entity.InterviewId, request.ClubId);
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

                // Kiểm tra trùng email trong cùng club
                if (!string.IsNullOrWhiteSpace(request.ApplicantEmail))
                {
                    var existingInterview = await _repo.GetByClubIdAndEmail(request.ClubId, request.ApplicantEmail);
                    if (existingInterview != null)
                        return ApiResponse<InterviewResponseDto>.Failure(409, "Email này đã đăng ký rồi");
                }

                var entity = _mapping.ToEntity(request, InterviewStatus.Registered, ApplicationType.Online);

                await _repo.AddAsync(entity);
                await _repo.SaveChangesAsync();

                _logger.LogInformation("Created online interview {InterviewId} for club {ClubId}", entity.InterviewId, request.ClubId);
                return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "Đăng ký thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi create web interview {Time}", DateTime.UtcNow);
                return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        // Cập nhật interview - chỉ cập nhật được khi status = Registered hoặc CheckedIn
        public async Task<ApiResponse<InterviewResponseDto>> UpdateAsync(int id, UpdateInterviewRequestDto request)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

                // Rule: Không sửa khi Done
                if (entity.Status == InterviewStatus.Done)
                    return ApiResponse<InterviewResponseDto>.Failure(409, "Không thể cập nhật khi đã hoàn thành");

                // Rule: Không sửa khi Cancelled
                if (entity.Status == InterviewStatus.Cancelled)
                    return ApiResponse<InterviewResponseDto>.Failure(409, "Không thể cập nhật khi đã hủy");

                // Rule: Không sửa khi NoShow
                if (entity.Status == InterviewStatus.NoShow)
                    return ApiResponse<InterviewResponseDto>.Failure(409, "Không thể cập nhật khi đã No-Show");

                _mapping.UpdateEntity(entity, request);

                await _repo.UpdateAsync(entity);
                await _repo.SaveChangesAsync();

                _logger.LogInformation("Updated interview {InterviewId}", id);
                return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi update interview {Time}", DateTime.UtcNow);
                return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        // Xóa interview - chỉ xóa được nếu chưa Done
        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse.Failure(404, "Không tìm thấy");

                // Rule: Không xóa khi Done
                if (entity.Status == InterviewStatus.Done)
                    return ApiResponse.Failure(409, "Không thể xóa khi đã hoàn thành");

                await _repo.DeleteAsync(entity);
                await _repo.SaveChangesAsync();

                _logger.LogInformation("Deleted interview {InterviewId}", id);
                return ApiResponse.Success("Xóa thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi delete interview {Time}", DateTime.UtcNow);
                return ApiResponse.Failure(500, ex.Message);
            }
        }

        // Lấy chi tiết interview
        public async Task<ApiResponse<InterviewResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

                return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi getById interview {Time}", DateTime.UtcNow);
                return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        // Lấy danh sách + phân trang + filter
        public async Task<PagedResponse<InterviewResponseDto>> GetAllInterviewsAsync(InterviewFilter filter)
        {
            try
            {
                // Nếu có filter ClubId, lấy interviews của club đó
                List<Interview> list;
                if (filter.ClubId > 0)
                {
                    list = await _repo.GetByClubIdAsync(filter.ClubId);
                }
                else
                {
                    list = await _repo.GetAllAsync();
                }

                if (filter.CampaignId.HasValue && filter.CampaignId.Value > 0)
                {
                    list = list.Where(x => x.CampaignId == filter.CampaignId.Value).ToList();
                }

                // Filter by keyword
                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                {
                    var keyword = filter.Keyword.ToLower();
                    list = list.Where(x => x.ApplicantName.ToLower().Contains(keyword)).ToList();
                }

                // Filter by status
                if (filter.Status.HasValue)
                {
                    list = list.Where(x => (int)x.Status == filter.Status.Value).ToList();
                }

                // Filter by result
                if (filter.Result.HasValue)
                {
                    list = list.Where(x => (int)x.Result == filter.Result.Value).ToList();
                }

                var total = list.Count;

                var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
                var size = filter.PageSize <= 0 ? 10 : filter.PageSize;

                var sortList = list.OrderByDescending(x => x.CreatedAt).ToList();

                var items = sortList.Skip((page - 1) * size).Take(size).ToList();

                return new PagedResponse<InterviewResponseDto>
                {
                    Items = _mapping.ToListResponse(items),
                    PageNumber = page,
                    PageSize = size,
                    TotalCount = total,
                    TotalPages = (int)Math.Ceiling(total / (double)size)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi get list interview {Time}", DateTime.UtcNow);
                throw;
            }
        }

        // Check-in ứng viên - chuyển từ Registered → CheckedIn
        public async Task<ApiResponse<InterviewResponseDto>> CheckInAsync(int id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

                // Rule: Chỉ check-in được khi status = Registered
                if (entity.Status != InterviewStatus.Registered)
                    return ApiResponse<InterviewResponseDto>.Failure(409, $"Không thể check-in khi trạng thái là {entity.Status}");

                _mapping.MapCheckIn(entity);

                await _repo.SaveChangesAsync();

                _logger.LogInformation("Checked in interview {InterviewId}", id);
                return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "Check-in thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi checkin {Time}", DateTime.UtcNow);
                return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        // Bắt đầu phỏng vấn - chuyển từ CheckedIn → Interviewing
        public async Task<ApiResponse<InterviewResponseDto>> StartAsync(int id, StartInterviewRequestDto request)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

                // Rule: Chỉ bắt đầu được khi status = CheckedIn
                if (entity.Status != InterviewStatus.CheckedIn)
                    return ApiResponse<InterviewResponseDto>.Failure(409, $"Không thể bắt đầu khi trạng thái là {entity.Status}. Phải Check-in trước");

                _mapping.MapStart(entity, request);

                await _repo.SaveChangesAsync();

                _logger.LogInformation("Started interview {InterviewId}", id);
                return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "Bắt đầu phỏng vấn");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi start interview {Time}", DateTime.UtcNow);
                return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        // Kết thúc phỏng vấn - chuyển từ Interviewing → Done
        public async Task<ApiResponse<InterviewResponseDto>> FinishAsync(int id, FinishInterviewRequestDto request)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

                // Rule: Chỉ kết thúc được khi status = Interviewing
                if (entity.Status != InterviewStatus.Interviewing)
                    return ApiResponse<InterviewResponseDto>.Failure(409, $"Không thể kết thúc khi trạng thái là {entity.Status}. Phải bắt đầu phỏng vấn trước");

                //// Rule: Phải có result (Pass/Fail)
                //if (request.Result < 1 || request.Result > 2)
                //    return ApiResponse<InterviewResponseDto>.Failure(400, "Result phải là 1 (Pass) hoặc 2 (Fail)");

                _mapping.MapFinish(entity, request);

                await _repo.SaveChangesAsync();

                _logger.LogInformation("Finished interview {InterviewId} with result {Result}", id, request.Result);
                return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "Hoàn thành");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi finish interview {Time}", DateTime.UtcNow);
                return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        // Đánh dấu no-show - chuyển sang NoShow
        public async Task<ApiResponse<InterviewResponseDto>> NoShowAsync(int id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

                // Rule: Chỉ No-Show được từ Registered hoặc CheckedIn
                if (entity.Status != InterviewStatus.Registered && entity.Status != InterviewStatus.CheckedIn)
                    return ApiResponse<InterviewResponseDto>.Failure(409, $"Không thể đánh dấu no-show khi trạng thái là {entity.Status}");

                _mapping.MapNoShow(entity, "Không đến");

                await _repo.SaveChangesAsync();

                _logger.LogInformation("Marked interview {InterviewId} as no-show", id);
                return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "No-show");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi noshow {Time}", DateTime.UtcNow);
                return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        // Hủy interview
        public async Task<ApiResponse<InterviewResponseDto>> CancelAsync(int id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

                // Rule: Không hủy được khi đã Done
                if (entity.Status == InterviewStatus.Done)
                    return ApiResponse<InterviewResponseDto>.Failure(409, "Không thể hủy khi đã hoàn thành");

                // Rule: Không hủy lần thứ 2
                if (entity.Status == InterviewStatus.Cancelled)
                    return ApiResponse<InterviewResponseDto>.Failure(409, "Interview đã được hủy rồi");

                _mapping.MapCancel(entity);

                await _repo.SaveChangesAsync();

                _logger.LogInformation("Cancelled interview {InterviewId}", id);
                return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "Đã hủy");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cancel interview {Time}", DateTime.UtcNow);
                return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ApiResponse<InterviewResponseDto>> UpdateResultAfterInterviewAsync(int id, UpdateInterviewAfterInterview request)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<InterviewResponseDto>.Failure(404, "Không tìm thấy");

                // Rule: Không hủy lần thứ 2
                if (entity.Status == InterviewStatus.Registered || entity.Status == InterviewStatus.CheckedIn)
                    return ApiResponse<InterviewResponseDto>.Failure(409, "Trạng thái không phù hợp");

                entity.Result = (InterviewResult)request.Result;
                entity.Evaluation = request.Evaluation;
                entity.Note = request.Note;

                await _repo.UpdateAsync(entity);
                await _repo.SaveChangesAsync();

                _logger.LogInformation("Update interview {InterviewId}", id);
                return ApiResponse<InterviewResponseDto>.Success(_mapping.ToResponse(entity), "Đã Update");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi Update interview {Time}", DateTime.UtcNow);
                return ApiResponse<InterviewResponseDto>.Failure(500, ex.Message);
            }
        }
    }
}