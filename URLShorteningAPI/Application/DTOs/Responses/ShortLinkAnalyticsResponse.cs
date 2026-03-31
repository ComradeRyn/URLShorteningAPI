namespace Application.DTOs.Responses;

public record ShortLinkAnalyticsResponse(
    int TotalVisits,
    string CreatedAt,
    string? LastVisitedAt);