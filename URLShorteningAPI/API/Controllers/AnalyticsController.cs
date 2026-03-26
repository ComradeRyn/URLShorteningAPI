using Application.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/analytics")]
[ApiController]
// [Authorize]
public class AnalyticsController : ControllerBase
{

    [HttpGet("{shortAlias}")]
    [ProducesResponseType(typeof(ShortLinkAnalyticsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShortLinkAnalyticsResponse>> GetShortLink(
        string shortAlias,
        string startDate,
        string endDate)
    {
        throw new NotImplementedException();
    }

    [HttpGet("visits")]
    [ProducesResponseType(typeof(VisitAnalyticsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<VisitAnalyticsResponse>> GetVisits(string startDate, string endDate)
    {
        throw new NotImplementedException();
    }
}