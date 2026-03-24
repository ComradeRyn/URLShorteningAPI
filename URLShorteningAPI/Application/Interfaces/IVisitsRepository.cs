using Application.DTOs.Responses;

namespace Application.Interfaces;

public interface IVisitsRepository
{
    Task<VisitsAnalyticsModel> GetAnalytics(DateTime startDate, DateTime endDate);
}