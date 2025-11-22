using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentClub.Application.DTOs.request;
using StudentClub.Application.IServices;

namespace StudentClub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubMembersController : ControllerBase
    {
        private readonly IClubMemberService _memberService;

        public ClubMembersController(IClubMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpPost]
        [Authorize(Roles = "admin, leader")]
        public async Task<IActionResult> CreateClubMember([FromBody] CreateClubMemberRequestDto createClubMemberRequestDto)
        {
            var result = await _memberService.CreateClubMemberAsync(createClubMemberRequestDto);
            return Ok(result);
        }

        [HttpGet("club/{clubId}")]
        [Authorize(Roles = "admin, leader")]
        public async Task<IActionResult> GetClubMembersByClubId(int clubId)
        {
            var result = await _memberService.GetAllClubMemberByClubIdAsync(clubId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClubMemberById(int id)
        {
            var result = await _memberService.GetClubMemberByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Không có thành viên nào" });
            }
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "admin, leader")]
        public async Task<IActionResult> GetAllClubMembers()
        {
            var result = await _memberService.GetAllClubMemberAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin, leader")]
        public async Task<IActionResult> UpdateClubMember(int id, [FromBody] CreateClubMemberRequestDto updateClubMemberRequestDto)
        {
            try
            {
                await _memberService.UpdateClubMemberAsync(id, updateClubMemberRequestDto);
                return Ok(new { message = "Cập nhật thành viên câu lạc bộ thành công" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
