using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.IServices;

namespace StudentClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClubsController : Controller
    {
        private readonly IClubService _clubService;

        public ClubsController(IClubService clubService)
        {
            _clubService = clubService;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateClub([FromBody] CreateClubRequestDto request)
        { 
            var result = await _clubService.CreateClubAsync(request);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetClubsAsync()
        {
            var result = await _clubService.GetAllClubAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClubAsync(int id)
        {
            var result = await _clubService.GetClubAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Không có club nào" });
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin, leader")]
        public async Task<IActionResult> UpdateClub([FromBody] UpdateClubRequestDto update)
        {
            try
            {
                await _clubService.UpdateClubAsync(update);
                return Ok(new { message = "Cập nhật trạng thái thành công" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra: " + ex.Message }); 
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            try
            {
                await _clubService.DeleteClubAsync(id);
                return Ok(new { message = "Xóa thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
