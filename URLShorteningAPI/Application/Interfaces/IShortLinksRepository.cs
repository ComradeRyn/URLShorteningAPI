using Application.DTOs.Responses;
using Domain.Models;

namespace Application.Interfaces;

public interface IShortLinksRepository
{
    Task<int> GetCount(DateTime? startDate, DateTime? endDate);
    Task<ShortLink?> GetByShortCode(string shortCode);
    Task<ShortLink?> GetByCustomAlias(string customAlias);
    Task<ShortLink> Add(
        string longUrl,
        string? customAlias,
        string? password);
    Task<ShortLinkAnalyticsModel> GetAnalytics(
        ShortLink shortLink,
        DateTime? startDate,
        DateTime? endDate);
}