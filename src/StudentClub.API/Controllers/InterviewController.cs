using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentClub.Application.DTOs;
using StudentClub.Application.IServices;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace StudentClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewsController : ControllerBase
    {
        private readonly IInterviewService _service;

        public InterviewsController(IInterviewService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "admin,leader")]
        public async Task<IActionResult> Create([FromBody] CreateInterviewRequestDto request)
        {
            try
            {
                var (userId, role) = GetUserContext();
                var result = await _service.CreateAsync(request, userId, role);
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
                var (userId, role) = GetUserContext();
                var result = await _service.UpdateAsync(id, request, userId, role);
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

        [HttpGet("club/{clubId}")]
        [Authorize(Roles = "admin,leader")]
        public async Task<IActionResult> GetByClub(int clubId)
        {
            try
            {
                var (userId, role) = GetUserContext();
                var result = await _service.GetByClubIdAsync(clubId, userId, role);
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



        // Hàm lấy userId và role từ JWT
        private (int userId, string role) GetUserContext()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
                throw new UnauthorizedAccessException("Token không hợp lệ");

            return (int.Parse(userIdClaim), roleClaim);
        }
    }
}
