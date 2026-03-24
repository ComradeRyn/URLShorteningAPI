namespace Application.DTOs.Responses;

public record VisitAnalyticsResponse(
    int TotalShortLinksCreate,
    int TotalVisits,
    IEnumerable<string> TopFiveUrls);