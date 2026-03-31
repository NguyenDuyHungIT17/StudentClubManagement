using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.EventRegistration;
using StudentClub.Application.DTOs.response.Event;
using StudentClub.Application.DTOs.response.EventRegistration;
using StudentClub.Application.IServices;
using StudentClub.Shared.ApiResponse;
using System.Security.Claims;
using System.Text.Json;

namespace StudentClub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventRegistrationsController : ControllerBase
    {
        private readonly IEventRegistrationService _eventRegistrationService;

        public EventRegistrationsController(IEventRegistrationService eventRegistrationService)
        {
            _eventRegistrationService = eventRegistrationService;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,leader")]
        public async Task<IActionResult> DeleteEventRegistration(int id)
        {
            var (userId, role) = GetUserContext();

            var result = await _eventRegistrationService.DeleteEventRegistration(id, role, userId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEventRegistration([FromBody] CreateEventRegistrationRequestDto request)
        {
            var (userId, _) = GetUserContext();

            var result = await _eventRegistrationService.CreateEventRegistrationWithUserAsync(request, userId);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("register-guest")]
        public async Task<IActionResult> RegisterGuest(CreateEventRegistrationRequestDto request)
        {
            var result = await _eventRegistrationService.CreateEventRegistrationGuestAsync(request);
            return Ok(result);
        }

        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "admin,leader")]
        public async Task<IActionResult> GetAllEventRegistrationsByEventId(int eventId, [FromQuery]EventRegistrationFilter filter)
        {
            var result = await _eventRegistrationService.GetAllEventRegistrationsByEventId(eventId, filter);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new
            {
                result.PageNumber,
                result.PageSize,
                result.TotalPages,
                result.TotalCount,
                result.HasPreviousPage,
                result.HasNextPage
            }));

            return Ok(ApiResponse<List<CreateEventRegistrationResponseDto>>.Success(result.Items));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _eventRegistrationService.GetById(id);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,leader,member")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateEventRegistrationRequestDto request)
        {
            var (userId, role) = GetUserContext();

            var result = await _eventRegistrationService.Update(id, request, role, userId);

            return Ok(result);
        }

        private (int userId, string role) GetUserContext()
        {
            var userIdOnToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleUserOnToken = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdOnToken) || string.IsNullOrEmpty(roleUserOnToken))
            {
                throw new UnauthorizedAccessException("Token không hợp lệ");
            }

            if (!int.TryParse(userIdOnToken, out int userIdFromToken))
            {
                throw new UnauthorizedAccessException("UserId trong token không hợp lệ");
            }

            return (userIdFromToken, roleUserOnToken.ToLower());
        }
    }
}