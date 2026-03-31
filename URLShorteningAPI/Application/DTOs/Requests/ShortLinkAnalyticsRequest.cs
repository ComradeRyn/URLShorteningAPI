namespace Application.DTOs.Requests;

public record ShortLinkAnalyticsRequest(
    string ShortAlias,
    string? StartDate,
    string? EndDate);