using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentClub.Application.DTOs;
using StudentClub.Application.DTOs.User;
using StudentClub.Application.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
        {
            var result = await _userService.CreateUserAsync(request);
            return Ok(result);
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserAsync(int userId)
        {
            try
            {
                var userIdOnToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleUserOnToken = User.FindFirst(ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(userIdOnToken) || string.IsNullOrEmpty(roleUserOnToken))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                if (!int.TryParse(userIdOnToken, out int userIdFromToken))
                {
                    return Unauthorized(new { message = "UserId trong token không hợp lệ" });
                }

                var user = await _userService.GetUserByIdAsync(userId, roleUserOnToken, userIdFromToken);
                
                if (user == null)
                {
                    return NotFound("Không tồn tại người dùng, hoặc bản không đủ quyền truy cập");
                }
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin, leader")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            try
            {
                var userIdOnToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleUserOnToken = User.FindFirst(ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(userIdOnToken) || string.IsNullOrEmpty(roleUserOnToken))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                if (!int.TryParse(userIdOnToken, out int userIdFromToken))
                {
                    return Unauthorized(new { message = "UserId trong token không hợp lệ" });
                }

                var users = await _userService.GetAllUsersAsync(userIdFromToken);
                return Ok(users);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)   
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequestDto request)
        {
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
            {
                return Unauthorized(new { message = "Token không hợp lệ" });
            }

            if (!int.TryParse(userIdClaim, out int userIdFromToken))
            {
                return Unauthorized(new { message = "UserId trong token không hợp lệ" });
            }

            string role = roleClaim;

            try
            {
                var result = await _userService.UpdateUserAsync(userIdFromToken, role, id, request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message); 
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
        [HttpPut("{id}/is-active")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateIsActiveUser(int id, [FromBody] UpdateIsActiveUserDto request)
        {
            try
            {
                await _userService.UpdateIsActiveUserAsync(request.isActive, id);
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
        [Authorize(Roles = "admin,leader")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
                return Unauthorized(new { message = "Token không hợp lệ" });

            int userIdFromToken = int.Parse(userIdClaim);
            string role = roleClaim;

            try
            {
                await _userService.DeleteUserAsync(userIdFromToken, role, id);
                return Ok(new { message = "User đã được xóa thành công" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message); // 403
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
