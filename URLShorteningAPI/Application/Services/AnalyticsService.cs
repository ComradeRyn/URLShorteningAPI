using System.Net;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;

namespace Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IShortLinksRepository _shortLinksRepository;
    private readonly IVisitsRepository _visitsRepository;

    public AnalyticsService(IShortLinksRepository shortLinksRepository, IVisitsRepository visitsRepository)
    {
        _shortLinksRepository = shortLinksRepository;
        _visitsRepository = visitsRepository;
    }
    
    public async Task<ApiResponse<ShortLinkAnalyticsResponse>> GetShortLink(ShortLinkAnalyticsRequest request)
    {
        var shortLink = await _shortLinksRepository.GetByShortCode(request.ShortAlias) ?? 
                        await _shortLinksRepository.GetByCustomAlias(request.ShortAlias);

        if (shortLink is null)
        {
            return new ApiResponse<ShortLinkAnalyticsResponse>(
                HttpStatusCode.NotFound, Messages.LinkNotFound);
        }

        var analytics = await _shortLinksRepository.GetAnalytics(
            shortLink,
            DateTime.Parse(request.StartDate).ToUniversalTime(),
            DateTime.Parse(request.EndDate).AddDays(1).ToUniversalTime());

        return new ApiResponse<ShortLinkAnalyticsResponse>(
            new ShortLinkAnalyticsResponse(
                analytics.TotalVisits,
                shortLink.CreationDate.ToLocalTime(),
                analytics.LastVisitedAt?.ToLocalTime()));
    }

    public async Task<ApiResponse<VisitAnalyticsResponse>> GetVisits(VisitAnalyticsRequest request)
    {
        var startDate = DateTime.Parse(request.StartDate).ToUniversalTime();
        var endDate = DateTime.Parse(request.EndDate).AddDays(1).ToUniversalTime();
        
        var visitAnalytics = await _visitsRepository.GetAnalytics(startDate, endDate);
        var totalShortLinksCreated = await _shortLinksRepository.GetCount(startDate, endDate);

        return new ApiResponse<VisitAnalyticsResponse>(new VisitAnalyticsResponse(
                totalShortLinksCreated,
                visitAnalytics.TotalVisits,
                visitAnalytics.TopFiveUrls));
    }
}