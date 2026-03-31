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
        var shortLink = await _shortLinksRepository.Get(request.ShortAlias);
        if (shortLink is null)
        {
            return new ApiResponse<ShortLinkAnalyticsResponse>(
                HttpStatusCode.NotFound, Messages.LinkNotFound);
        }
        
        if (!TryParseDate(request.StartDate, out var startDate) || !TryParseDate(request.EndDate, out var endDate))
        {
            return new ApiResponse<ShortLinkAnalyticsResponse>(
                HttpStatusCode.BadRequest, Messages.InvalidDateFormat);
        }

        var analytics = await _shortLinksRepository.GetAnalytics(
            shortLink,
            startDate,
            endDate?.AddDays(1));

        return new ApiResponse<ShortLinkAnalyticsResponse>(
            new ShortLinkAnalyticsResponse(
                analytics.TotalVisits,
                shortLink.CreationDate.ToLocalTime(),
                analytics.LastVisitedAt?.ToLocalTime()));
    }

    public async Task<ApiResponse<VisitAnalyticsResponse>> GetVisits(VisitAnalyticsRequest request)
    {
        if (!TryParseDate(request.StartDate, out var startDate) || !TryParseDate(request.EndDate, out var endDate))
        {
            return new ApiResponse<VisitAnalyticsResponse>(
                HttpStatusCode.BadRequest, Messages.InvalidDateFormat);
        }
        
        var visitAnalytics = await _visitsRepository.GetAnalytics(
            startDate,
            endDate?.AddDays(1));
        
        var totalShortLinksCreated = await _shortLinksRepository.GetCount(
            startDate,
            endDate?.AddDays(1));

        return new ApiResponse<VisitAnalyticsResponse>(new VisitAnalyticsResponse(
                totalShortLinksCreated,
                visitAnalytics.TotalVisits,
                visitAnalytics.TopFiveUrls));
    }

    private static bool TryParseDate(string? inputDate, out DateTime? parsedDate)
    {
        if (inputDate is null)
        {
            parsedDate = null;
            return true;
        }

        var isSuccess = DateTime.TryParseExact(
            inputDate,
            ValidFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal,
            out var parsedNonNullDate);

         parsedDate = parsedNonNullDate;

         return isSuccess;
    }
}