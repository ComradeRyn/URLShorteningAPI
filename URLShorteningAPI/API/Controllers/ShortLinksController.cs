using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/shortlinks")]
[ApiController]
// [Authorize]
public class ShortLinksController : ControllerBase
{
    private readonly IShortLinksService _shortLinksService;

    public ShortLinksController(IShortLinksService shortLinksService)
    {
        _shortLinksService = shortLinksService;
    }
    
    // TODO: add comment documenting endpoints
    [HttpPost]
    [ProducesResponseType(typeof(ShortUrlResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ShortUrlResponse>> ShortenUrl(CreationRequest request)
    {
        var response = await _shortLinksService.ShortenUrl(request);
        if (!response.IsSuccess)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(response.Content);
    }

    [HttpGet("{shortAlias}")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResolveUrl(string shortAlias)
    {
        var response = await _shortLinksService.ResolveUrl(shortAlias);
        if (!response.IsSuccess)
        {
            return BadRequest(response.ErrorMessage);
        }
        
        return Redirect(response.Content!);
    }

    [HttpPost("verification")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> VerifyPassword(VerifyPasswordRequest request)
    {
        var response = await _shortLinksService.VerifyPassword(request);
        if (!response.IsSuccess)
        {
            // TODO: look into sending forbidden status code
            // return Forbid(response.ErrorMessage); <- doesn't work, but seems more coherent
            return StatusCode(StatusCodes.Status403Forbidden, response.ErrorMessage);
        }

        return Redirect(response.Content!);
    }
}