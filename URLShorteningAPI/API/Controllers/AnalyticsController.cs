using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/analytics")]
[ApiController]
// [Authorize]
public class AnalyticsController : ControllerBase
{
    private IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("{shortAlias}")]
    [ProducesResponseType(typeof(ShortLinkAnalyticsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShortLinkAnalyticsResponse>> GetShortLink(
        string shortAlias,
        string startDate,
        string endDate)
    {
        var response = await _analyticsService.GetShortLink(
            new ShortLinkAnalyticsRequest(
                shortAlias,
                startDate,
                endDate));

        if (!response.IsSuccess)
        {
            return NotFound(response.ErrorMessage);
        }

        return Ok(response.Content);
    }

    [HttpGet("visits")]
    [ProducesResponseType(typeof(VisitAnalyticsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<VisitAnalyticsResponse>> GetVisits(string startDate, string endDate)
    {
        throw new NotImplementedException();
    }
}