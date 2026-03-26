using Application.DTOs.Responses;
using Application.Interfaces;

namespace Infrastructure.Repositories;

public class VisitsRepository : IVisitsRepository
{
    public Task<VisitsAnalyticsModel> GetAnalytics(DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }
}