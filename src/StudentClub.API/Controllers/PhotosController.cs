using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.IServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentClub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IPhotoService _photoService;

        public PhotosController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        /// <summary>
        /// Upload một ảnh mới lên Cloudinary và lưu vào DB
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadPhoto([FromForm] UploadPhotoRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _photoService.UploadPhotoAsync(request);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePhoto(int id, [FromForm] UpdatePhotoRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get userId from token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _photoService.UpdatePhotoAsync(id, request, userId);

            if (result.IsSuccess)
                return Ok(result);

            return StatusCode(result.Status, result);
        }

        [HttpGet("clubmember/{clubMemberId}")]
        public async Task<IActionResult> GetPhotosByClubMemberId(int clubMemberId)
        {
            var result = await _photoService.GetPhotosByClubMemberIdAsync(clubMemberId);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Xóa ảnh bằng PhotoId
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var result = await _photoService.DeletePhotoAsync(id);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result); // Trả về 400 kèm câu thông báo lỗi đã viết ở Service
        }

        /// <summary>
        /// Lấy toàn bộ ảnh của một Sự kiện
        /// </summary>
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetPhotosByEvent(int eventId)
        {
            var result = await _photoService.GetPhotosByEventIdAsync(eventId);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result); // Dùng 404 cho trường hợp không tìm thấy dữ liệu
        }

        /// <summary>
        /// Lấy toàn bộ ảnh của một Câu lạc bộ
        /// </summary>
        [HttpGet("club/{clubId}")]
        public async Task<IActionResult> GetPhotosByClub(int clubId)
        {
            var result = await _photoService.GetPhotosByClubIdAsync(clubId);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Lấy toàn bộ ảnh của một Người dùng (User)
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPhotosByUser(int userId)
        {
            var result = await _photoService.GetPhotosByUserIdAsync(userId);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }
    }
}