using Application.DTOs.Requests;
using Application.DTOs.Responses;

namespace Application.Interfaces;

public interface IShortLinksService
{
    Task<ApiResponse<ShortUrlResponse>> ShortenUrl(CreationRequest request);
    ApiResponse<string> ResolveUrl(string shortAlias);
    ApiResponse<string> VerifyPassword(VerifyPasswordRequest request);
}