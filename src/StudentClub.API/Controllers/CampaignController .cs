using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using StudentClub.Application.DTOs.Filter;
using StudentClub.Application.DTOs.request.Campaign;
using StudentClub.Application.IServices;
using StudentClub.Shared.ApiResponse;
using StudentClub.Application.DTOs.response.Campaign;

namespace StudentClub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCampaign([FromBody] CampaignRequest request)
        {
            var result = await _campaignService.CreateCampaignAsync(request);
            return StatusCode(result.Status, result);
        }

        [HttpGet("{campaignId}")]
        public async Task<IActionResult> GetCampaignById(int campaignId)
        {
            var result = await _campaignService.GetCampaignByIdAsync(campaignId);
            return StatusCode(result.Status, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCampaigns([FromQuery] CampaignFilterRequest filter)
        {
            var result = await _campaignService.GetCampaignsAsync(filter);

            // Add pagination metadata to header
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new
            {
                result.PageNumber,
                result.PageSize,
                result.TotalPages,
                result.TotalCount,
                result.HasPreviousPage,
                result.HasNextPage
            }));

            return Ok(ApiResponse<List<CampaignResponse>>.Success(result.Items));
        }

        [HttpPut("{campaignId}")]
        public async Task<IActionResult> UpdateCampaign(int campaignId, [FromBody] CampaignRequest request)
        {
            var result = await _campaignService.UpdateCampaignAsync(campaignId, request);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("{campaignId}")]
        public async Task<IActionResult> DeleteCampaign(int campaignId)
        {
            var result = await _campaignService.DeleteCampaignAsync(campaignId);
            return StatusCode(result.Status, result);
        }
    }
}