using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.IServices;
using System.Security.Claims;

namespace StudentClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController: ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpPost]
        [Authorize(Roles = "admin, leader")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequestDto request)
        {
            try
            {
                var (userId, role) = GetUserContext();
                var result = await _eventService.CreateEventAsync(request, userId, role);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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

        [HttpPut("{eventId}")]
        [Authorize (Roles = "admin, leader")]
        public async Task<IActionResult> UpdateEvent([FromBody] UpdateEventRequestDto request, int eventId)
        {
            try
            {
                var (userId, role) = GetUserContext();
                var result = await _eventService.UpdateEventAsync(request, eventId, userId, role);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
        [HttpGet("/publicEvents")]
        public async Task<IActionResult> GetPublicEvents()
        {
            try
            {
                var result = await _eventService.GetPublicEventsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("/publicEvents/club")]
        public async Task<IActionResult> GetEventsByClubId([FromQuery] int clubId)
        {
            try
            {
                var result = await _eventService.GetPublicEventsByClubIdAsync(clubId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventById(int eventId)
        {
            try
            {
                var result = await _eventService.GetEventByIdAsync(eventId);
                return Ok(result);
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var (userId, role) = GetUserContext();
                var result = await _eventService.GetAllEventsAsync(role, userId);
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, leader")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _eventService.DeleteEvent(id);
            return Ok(new { message = "Xóa thành công" });
        }

        [HttpGet("event/{clubId}")]
        [Authorize]
        public async Task<IActionResult> GetEventsByClubIdAsync(int clubId)
        {
            try
            {
                var (userId, role) = GetUserContext();
                var result = await _eventService.GetEventsByClubIdAsync(userId);
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
