using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentClub.Application.DTOs;
using StudentClub.Application.DTOs.Clubs;
using StudentClub.Application.Services;

namespace StudentClub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubMembersController : ControllerBase
    {
        private readonly ClubMemberService _memberService;

        public ClubMembersController(ClubMemberService memberService)
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
    }
}
