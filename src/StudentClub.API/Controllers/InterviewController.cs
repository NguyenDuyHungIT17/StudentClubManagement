using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.Request.Interview;
using StudentClub.Application.IServices;
using System.Security.Claims;

namespace StudentClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewsController : ControllerBase
    {
        private readonly IInterviewService _service;
        private readonly IEmailService _emailService;

        public InterviewsController(IInterviewService service, IEmailService emailService)
        {
            _service = service;
            _emailService = emailService;
        }

        //tạo trực tiếp walk in
        [HttpPost]
        [Authorize(Roles = "admin,leader")]
        public async Task<IActionResult> Create([FromBody] CreateInterviewRequestDto request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //create từ web - khách
        [HttpPost("web")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateWeb([FromBody] CreateInterviewRequestDto request)
        {
            try
            {
                var result = await _service.CreateWebAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,leader")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInterviewRequestDto request)
        {
            try
            {
                var result = await _service.UpdateAsync(id, request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //email
        [HttpPost("club/{clubId}/send-email/{resultType}")]
        [Authorize(Roles = "admin,leader")]
        public async Task<IActionResult> SendInterviewResultEmail(int clubId, int resultType)
        {
            try
            {
                var (userId, role) = GetUserContext();

                await _emailService.SendInterviewResultEmailAsync(clubId, resultType);
                return Ok($"Đã gửi email cho các bạn {resultType}.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Lấy danh sách interview + filter + phân trang
        [HttpGet]
        [Authorize(Roles = "admin,leader,member")]
        public async Task<IActionResult> GetAll([FromQuery] InterviewFilter filter)
        {
            try
            {
                var result = await _service.GetAllInterviewsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Lấy chi tiết interview
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,leader,member")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Check-in ứng viên
        [HttpPost("{id}/checkin")]
        [Authorize(Roles = "admin,leader")]
        public async Task<IActionResult> CheckIn(int id)
        {
            try
            {
                var result = await _service.CheckInAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Bắt đầu phỏng vấn
        [HttpPost("{id}/start")]
        [Authorize(Roles = "admin,leader,member")]
        public async Task<IActionResult> Start(int id, [FromBody] StartInterviewRequestDto request)
        {
            try
            {
                var result = await _service.StartAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Kết thúc phỏng vấn
        [HttpPost("{id}/finish")]
        [Authorize(Roles = "admin,leader,member")]
        public async Task<IActionResult> Finish(int id, [FromBody] FinishInterviewRequestDto request)
        {
            try
            {
                var result = await _service.FinishAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //No-show
        [HttpPost("{id}/noshow")]
        [Authorize(Roles = "admin,leader")]
        public async Task<IActionResult> NoShow(int id)
        {
            try
            {
                var result = await _service.NoShowAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Hủy phỏng vấn
        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "admin,leader")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _service.CancelAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Xóa interview
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private (int userId, string role) GetUserContext()
        {
            var userIdOnToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleUserOnToken = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdOnToken) || string.IsNullOrEmpty(roleUserOnToken))
            {
                Unauthorized(new { message = "Token không hợp lệ" });
            }

            if (!int.TryParse(userIdOnToken, out int userIdFromToken))
            {
                Unauthorized(new { message = "UserId trong token không hợp lệ" });
            }

            return (userIdFromToken, roleUserOnToken);
        }
    }
}