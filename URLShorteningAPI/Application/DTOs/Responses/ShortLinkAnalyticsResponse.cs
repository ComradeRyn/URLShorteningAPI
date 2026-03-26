namespace Application.DTOs.Responses;

public record ShortLinkAnalyticsResponse(
    int TotalVisits,
    DateTime CreatedAt,
    DateTime? LastVisitedAt);