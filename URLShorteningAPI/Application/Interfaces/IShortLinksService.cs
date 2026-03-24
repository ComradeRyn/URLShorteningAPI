using Application.DTOs.Requests;
using Application.DTOs.Responses;

namespace Application.Interfaces;

public interface IShortLinksService
{
    Task<ApiResponse<ShortUrlResponse>> ShortenUrl(CreationRequest request);
    Task<ApiResponse<string>> ResolveUrl(string shortAlias);
    Task<ApiResponse<string>> VerifyPassword(VerifyPasswordRequest request);
}