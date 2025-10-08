using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentClub.Application.DTOs;
using StudentClub.Application.IServices;
using StudentClub.Domain.Entities;
using StudentClub.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        [Authorize(Roles = "admin, leader")]
        public async Task<IActionResult> DeleteEventRegistration(int id)
        {
            var (userId, role) = GetUserContext();
            try
            {
                await _eventRegistrationService.DeleteEventRegistration(id, role, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateEventRegistration([FromBody] CreateEventRegistrationRequestDto request)
        {
            try
            {
                var result = await _eventRegistrationService.CreateEventRegistrationAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "admin, leader")]
        public async Task<IActionResult> GetAllEventRegistrationsByEventId(int eventId)
        {
            try
            {
                var result = await _eventRegistrationService.GetAllEventRegistrationsByEventId(eventId);
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
