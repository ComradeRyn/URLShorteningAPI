using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/shortlinks")]
[ApiController]
[Authorize]
public class ShortLinksController : ControllerBase
{
    private readonly IShortLinksService _shortLinksService;

    public ShortLinksController(IShortLinksService shortLinksService)
    {
        _shortLinksService = shortLinksService;
    }
    
    /// <summary>
    /// Shortens a complete URL
    /// </summary>
    /// <param name="request">A record containing a required LongUrl of the intended destination, along with
    /// an optional custom alias and password</param>
    /// <returns>A shortened url ending with either the requested custom alias or an auto generated shortcode</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ShortUrlResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ShortUrlResponse>> ShortenUrl(CreationRequest request)
    {
        var response = await _shortLinksService.ShortenUrl(request);
        if (!response.IsSuccess)
        {
            return StatusCode((int)response.StatusCode, response.ErrorMessage);
        }

        return Ok(response.Content);
    }

    /// <summary>
    /// Redirects to the requested short alias's referenced URL
    /// </summary>
    /// <param name="shortAlias">A string of either an auto-generated shortcode or a user requested custom alias</param>
    /// <returns>A redirection to the requested URL or verification site depending on if request required
    /// a password</returns>
    [AllowAnonymous]
    [HttpGet("{shortAlias}")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResolveUrl(string shortAlias)
    {
        var response = await _shortLinksService.ResolveUrl(shortAlias);
        if (!response.IsSuccess)
        {
            return StatusCode((int)response.StatusCode, response.ErrorMessage);
        }
        
        return Redirect(response.Content!);
    }

    /// <summary>
    /// Verifies an inputted password for a specified ShortLink
    /// </summary>
    /// <param name="request">A record containing a short alias and password</param>
    /// <returns>A redirection to the requested URL</returns>
    [HttpPost("verification")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> VerifyPassword(VerifyPasswordRequest request)
    {
        var response = await _shortLinksService.VerifyPassword(request);
        if (!response.IsSuccess)
        {
            return StatusCode((int)response.StatusCode, response.ErrorMessage);
        }

        return Redirect(response.Content!);
    }
}