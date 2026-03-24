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
    private const string CustomAliasRegexp = @"[\s\/]";
    private readonly IShortLinksRepository _shortLinksRepository;
    private readonly IConfiguration _configuration;

    public ShortLinksService(IShortLinksRepository shortLinksRepository, IConfiguration configuration)
    {
        _shortLinksRepository = shortLinksRepository;
        _configuration = configuration;
    }
    
    public async Task<ApiResponse<ShortUrlResponse>> ShortenUrl(CreationRequest request)
    {
        if (request.CustomAlias is not null)
        {
            if (ValidateCustomAlias(request.CustomAlias))
            {
                return new ApiResponse<ShortUrlResponse>(
                    HttpStatusCode.BadRequest, Messages.InvalidCustomAlias);
            }

            // Edge case, what if the customAlias is the same as an existing shortCode?
            if (await _shortLinksRepository.GetByCustomAlias(request.CustomAlias) is not null)
            {
                return new ApiResponse<ShortUrlResponse>(
                    HttpStatusCode.BadRequest, Messages.CustomAliasTaken);
            }
        }

        var shortLink = await _shortLinksRepository.Add(request.LongUrl,
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
            return new ApiResponse<string>($"{_configuration["PasswordValidationWebpage"]}?{shortAlias}");
        }

        return new ApiResponse<string>(shortLink.LongUrl);
    }

    public ApiResponse<string> VerifyPassword(VerifyPasswordRequest request)
    {
        throw new NotImplementedException();
    }

    private bool ValidateCustomAlias(string customAlias)
        => Regex.IsMatch(customAlias, CustomAliasRegexp);
}