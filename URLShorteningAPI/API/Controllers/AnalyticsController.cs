using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/analytics")]
[ApiController]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    // TODO: figure out specific date format
    /// <summary>
    /// Provides analytics on a requested ShortLink within a specified date range
    /// </summary>
    /// <param name="shortAlias">The short code or custom alias</param>
    /// <param name="startDate">The beginning of the date range</param>
    /// <param name="endDate">The ending of the date range</param>
    /// <returns>The total visits, creation date, and latest visit date</returns>
    [HttpGet("{shortAlias}")]
    [ProducesResponseType(typeof(ShortLinkAnalyticsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
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
            return StatusCode((int)response.ErrorCode!, response.ErrorMessage);
        }

        return Ok(response.Content);
    }

    /// <summary>
    /// Provides analytics on all visits within a specified date range
    /// </summary>
    /// <param name="startDate">The beginning of the date range</param>
    /// <param name="endDate">The ending of the date range</param>
    /// <returns>The total number of ShortLinks that were created, the total number of visits that have
    /// occured, and the top five Urls</returns>
    [HttpGet("visits")]
    [ProducesResponseType(typeof(VisitAnalyticsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VisitAnalyticsResponse>> GetVisits(string startDate, string endDate)
    {
        var response = await _analyticsService.GetVisits(new VisitAnalyticsRequest(startDate, endDate));
        if (!response.IsSuccess)
        {
            return StatusCode((int)response.ErrorCode!, response.ErrorMessage);
        }

        return response.Content!;
    }
}