namespace Application.DTOs.Responses;

public record VisitsAnalyticsModel(IEnumerable<string> TopFiveUrls, int TotalVisits);