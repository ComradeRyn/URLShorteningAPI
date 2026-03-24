using Application.DTOs.Requests;
using Application.DTOs.Responses;

namespace Application.Interfaces;

public interface IAnalyticsService
{
    Task<ApiResponse<ShortLinkAnalyticsResponse>> GetShortLink(ShortLinkAnalyticsRequest request);
    Task<ApiResponse<VisitAnalyticsResponse>> GetVisits(VisitAnalyticsRequest request);
}