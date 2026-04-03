using System.Net;
using System.Text.RegularExpressions;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class ShortLinksService : IShortLinksService
{
    private const string CustomAliasRegexp = @"^[\w\-_.~]*$";
    private readonly IShortLinksRepository _shortLinksRepository;
    private readonly IVisitsRepository _visitsRepository;
    private readonly IShortCodesService _shortCodesService;
    private readonly IPasswordHasher<ShortLink> _passwordHasher;
    private readonly IConfiguration _configuration;
    
    public ShortLinksService(
        IShortLinksRepository shortLinksRepository,
        IVisitsRepository visitsRepository,
        IConfiguration configuration,
        IShortCodesService shortCodesService,
        IPasswordHasher<ShortLink> passwordHasher)
    {
        _shortLinksRepository = shortLinksRepository;
        _configuration = configuration;
        _visitsRepository = visitsRepository;
        _shortCodesService = shortCodesService;
        _passwordHasher = passwordHasher;
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
            
            if (_shortCodesService.Decode(request.CustomAlias) is not null ||
                await _shortLinksRepository.Get(request.CustomAlias) is not null)
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

        var password = request.Password;
        if (password is not null)
        { 
            password = _passwordHasher.HashPassword(null!, password);
        }

        var shortLink = await _shortLinksRepository.Add(
            longUrl,
            request.CustomAlias,
            password);

        return new ApiResponse<ShortUrlResponse>(
            new ShortUrlResponse($"{_configuration["ShortLinkUrl"]}{shortLink.CustomAlias ?? shortLink.ShortCode!}"));
    }

    public async Task<ApiResponse<string>> ResolveUrl(string shortAlias)
    {
        var shortLink = await _shortLinksRepository.Get(shortAlias);
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
        var shortLink = await _shortLinksRepository.Get(request.ShortAlias);
        var verificationResult = _passwordHasher.VerifyHashedPassword(
            null!,
            shortLink!.Password!,
            request.Password);
        
        if (verificationResult is PasswordVerificationResult.Failed) 
        {
            return new ApiResponse<string>(HttpStatusCode.Forbidden, Messages.IncorrectPassword);
        }

        await _visitsRepository.Add(shortLink.Id);
        
        return new ApiResponse<string>(shortLink.LongUrl);
    }

    private static bool ValidateCustomAlias(string customAlias)
        => Regex.IsMatch(customAlias, CustomAliasRegexp);
}