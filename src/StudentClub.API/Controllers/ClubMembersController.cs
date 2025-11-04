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

       
    }
}
