using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentClub.Application.DTOs;
using StudentClub.Application.DTOs.Clubs;
using StudentClub.Application.DTOs.User;
using StudentClub.Application.IServices;
using StudentClub.Application.Services;
using StudentClub.Domain.Entities;
using StudentClub.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClubsController : Controller
    {
        private ClubService _clubService;

        public ClubsController(ClubService clubService)
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
