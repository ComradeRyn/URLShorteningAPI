using System.Net;
using System.Text.RegularExpressions;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;

namespace Application.Services;

public class ShortLinksService : IShortLinksService
{
    private const string CustomAliasRegexp = @"[\s\/]";
    private readonly IShortLinksRepository _shortLinksRepository;

    public ShortLinksService(IShortLinksRepository shortLinksRepository)
    {
        _shortLinksRepository = shortLinksRepository;
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

        return new ApiResponse<ShortUrlResponse>(
            HttpStatusCode.OK,
            new ShortUrlResponse($"https://tpt.link/{urlTail}"));
    }

    public ApiResponse<string> ResolveUrl(string shortAlias)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<string> VerifyPassword(VerifyPasswordRequest request)
    {
        throw new NotImplementedException();
    }

    private bool ValidateCustomAlias(string customAlias)
        => Regex.IsMatch(customAlias, CustomAliasRegexp);
}