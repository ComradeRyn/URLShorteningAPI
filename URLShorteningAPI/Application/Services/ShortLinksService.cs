using System.Net;
using System.Text.RegularExpressions;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class ShortLinksService : IShortLinksService
{
    private const string CustomAliasRegexp = @"^[\w\-_.~]*$";
    private readonly IShortLinksRepository _shortLinksRepository;
    private readonly IVisitsRepository _visitsRepository;
    private readonly IConfiguration _configuration;

    public ShortLinksService(
        IShortLinksRepository shortLinksRepository,
        IVisitsRepository visitsRepository,
        IConfiguration configuration)
    {
        _shortLinksRepository = shortLinksRepository;
        _configuration = configuration;
        _visitsRepository = visitsRepository;
    }
    
    public async Task<ApiResponse<ShortUrlResponse>> ShortenUrl(CreationRequest request)
    {
        if (request.CustomAlias is not null)
        {
            if (!ValidateCustomAlias(request.CustomAlias))
            {
                return new ApiResponse<ShortUrlResponse>(
                    HttpStatusCode.BadRequest, Messages.InvalidCustomAlias);
            }
            
            if (await _shortLinksRepository.GetByCustomAlias(request.CustomAlias) is not null ||
                await _shortLinksRepository.GetByShortCode(request.CustomAlias) is not null)
            {
                return new ApiResponse<ShortUrlResponse>(
                    HttpStatusCode.BadRequest, Messages.CustomAliasTaken);
            }
        }
        
        var longUrl = request.LongUrl;
        if (!longUrl.StartsWith("http://") && !longUrl.StartsWith("https://") )
        {
            longUrl = "https://" + longUrl;
        }

        var shortLink = await _shortLinksRepository.Add(
            longUrl,
            request.CustomAlias,
            request.Password);

        var urlTail = shortLink.CustomAlias ?? shortLink.ShortCode!;

        return new ApiResponse<ShortUrlResponse>(new ShortUrlResponse($"https://tpt.link/{urlTail}"));
    }

    public async Task<ApiResponse<string>> ResolveUrl(string shortAlias)
    {
        var shortLink = await _shortLinksRepository.GetByShortCode(shortAlias) ?? 
                        await _shortLinksRepository.GetByCustomAlias(shortAlias);

        if (shortLink is null)
        {
            return new ApiResponse<string>(HttpStatusCode.NotFound, Messages.LinkNotFound);
        }

        if (shortLink.Password is not null)
        {
            return new ApiResponse<string>(
                $"{_configuration["PasswordValidationWebpage"]}?shortAlias={shortAlias}");
        }
        
        await _visitsRepository.Add(shortLink.Id);
        
        return new ApiResponse<string>(shortLink.LongUrl);
    }

    public async Task<ApiResponse<string>> VerifyPassword(VerifyPasswordRequest request)
    {
        var shortLink = await _shortLinksRepository.GetByShortCode(request.ShortAlias) ?? 
                        await _shortLinksRepository.GetByCustomAlias(request.ShortAlias);

        if (request.Password != shortLink!.Password)
        {
            return new ApiResponse<string>(HttpStatusCode.Forbidden, Messages.IncorrectPassword);
        }

        await _visitsRepository.Add(shortLink.Id);
        
        return new ApiResponse<string>(shortLink.LongUrl);
    }

    private static bool ValidateCustomAlias(string customAlias)
        => Regex.IsMatch(customAlias, CustomAliasRegexp);
}