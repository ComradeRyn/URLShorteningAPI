namespace Application.DTOs.Responses;

public record VisitAnalyticsResponse(
    int TotalShortLinksCreated,
    int TotalVisits,
    IEnumerable<string> TopFiveUrls);