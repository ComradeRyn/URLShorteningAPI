using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;

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
    
    public Task<ApiResponse<ShortLinkAnalyticsResponse>> GetShortLink(ShortLinkAnalyticsRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<VisitAnalyticsResponse>> GetVisits(VisitAnalyticsRequest request)
    {
        throw new NotImplementedException();
    }
}