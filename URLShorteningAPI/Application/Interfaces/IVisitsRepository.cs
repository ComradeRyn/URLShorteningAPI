using Application.DTOs.Responses;
using Domain.Models;

namespace Application.Interfaces;

public interface IVisitsRepository
{
    Task<VisitsAnalyticsModel> GetAnalytics(DateTime startDate, DateTime endDate);
    Task<Visit> Add(long parent);
}