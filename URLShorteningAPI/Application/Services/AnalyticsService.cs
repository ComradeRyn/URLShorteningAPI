using System.Net;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;

namespace Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IShortLinksRepository _shortLinksRepository;
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsService(IShortLinksRepository shortLinksRepository, IAnalyticsService analyticsService)
    {
        _shortLinksRepository = shortLinksRepository;
        _analyticsService = analyticsService;
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
        
        // Input format should be "yyyyMMddHH"
        var startDate = DateTime.Parse(request.StartDate);
        var endDate = DateTime.Parse(request.EndDate);

        var query = shortLink
            .Visits
            .AsQueryable()
            .Where(visit => visit.Date >= startDate && visit.Date <= endDate);

        // Question: what happens if the link was never visited?
        var totalVisits = query.Count();
        var lastVisitedAt = query.MaxBy(visit => visit.Date)?.Date;

        return new ApiResponse<ShortLinkAnalyticsResponse>(
            new ShortLinkAnalyticsResponse(
                totalVisits,
                shortLink.CreationDate,
                lastVisitedAt));
    }

    public Task<ApiResponse<VisitAnalyticsResponse>> GetVisits(VisitAnalyticsRequest request)
    {
        throw new NotImplementedException();
    }
}