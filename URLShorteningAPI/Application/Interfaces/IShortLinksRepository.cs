using Application.DTOs.Responses;
using Domain.Models;

namespace Application.Interfaces;

public interface IShortLinksRepository
{
    Task<int> GetCount(DateTime? startDate, DateTime? endDate);
    Task<ShortLink?> Get(string shortAlias);
    Task<ShortLink> Add(
        string longUrl,
        string? customAlias,
        string? password);
    Task<ShortLinkAnalyticsModel> GetAnalytics(
        ShortLink shortLink,
        DateTime? startDate,
        DateTime? endDate);
}