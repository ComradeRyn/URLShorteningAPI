using System.Globalization;
using System.Net;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;

namespace Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private const string ValidFormat = "yyyy-MM-dd";
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
        
        if (!TryParseDate(request.StartDate, out var startDate)
            || !TryParseDate(request.EndDate, out var endDate))
        {
            return new ApiResponse<ShortLinkAnalyticsResponse>(
                HttpStatusCode.BadRequest, Messages.InvalidDateFormat);
        }

        var analytics = await _shortLinksRepository.GetAnalytics(
            shortLink,
            startDate.ToUniversalTime(),
            endDate.AddDays(1).ToUniversalTime());

        return new ApiResponse<ShortLinkAnalyticsResponse>(
            new ShortLinkAnalyticsResponse(
                analytics.TotalVisits,
                shortLink.CreationDate.ToLocalTime(),
                analytics.LastVisitedAt?.ToLocalTime()));
    }

    public async Task<ApiResponse<VisitAnalyticsResponse>> GetVisits(VisitAnalyticsRequest request)
    {
        if (!TryParseDate(request.StartDate, out var startDate)
            || !TryParseDate(request.EndDate, out var endDate))
        {
            return new ApiResponse<VisitAnalyticsResponse>(
                HttpStatusCode.BadRequest, Messages.InvalidDateFormat);
        }
        
        // TODO: Ask about this, and see if there is a better wayy
        var visitAnalytics = await _visitsRepository.GetAnalytics(
            startDate.ToUniversalTime(),
            endDate.AddDays(1).ToUniversalTime());
        var totalShortLinksCreated = await _shortLinksRepository.GetCount(
            startDate.ToUniversalTime(),
            endDate.AddDays(1).ToUniversalTime());

        return new ApiResponse<VisitAnalyticsResponse>(new VisitAnalyticsResponse(
                totalShortLinksCreated,
                visitAnalytics.TotalVisits,
                visitAnalytics.TopFiveUrls));
    }

    private bool TryParseDate(string inputDate,out DateTime parsedDate)
        => DateTime.TryParseExact(
            inputDate,
            ValidFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out parsedDate);
}