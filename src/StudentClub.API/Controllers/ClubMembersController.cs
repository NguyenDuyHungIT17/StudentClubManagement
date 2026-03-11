using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.ClubMember;
using StudentClub.Application.DTOs.response.Club;
using StudentClub.Application.DTOs.response.ClubMember;
using StudentClub.Application.IServices;
using StudentClub.Shared.ApiResponse;
using System.Text.Json;

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
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "admin, leader")]
        public async Task<IActionResult> GetAllClubMembers([FromQuery] ClubMemberFilter filter)
        {
            var result = await _memberService.GetAllClubMemberAsync(filter);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new
            {
                result.PageNumber,
                result.PageSize,
                result.TotalPages,
                result.TotalCount,
                result.HasPreviousPage,
                result.HasNextPage
            }));

            return Ok(ApiResponse<List<CreateClubMemberResponseDto>>.Success(result.Items));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin, leader")]
        public async Task<IActionResult> UpdateClubMember(int id, [FromBody] CreateClubMemberRequestDto updateClubMemberRequestDto)
        {
            try
            {
                var result = await _memberService.UpdateClubMemberAsync(id, updateClubMemberRequestDto);
                return Ok(result);
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, leader")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _memberService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
