using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VisitsRepository : IVisitsRepository
{
    private readonly UrlShorteningContext _context;

    public VisitsRepository(UrlShorteningContext context)
    {
        _context = context;
    }

    public async Task<Visit> Add(ShortLink parent)
    {
        var visit = new Visit
        {
            Date = DateTime.UtcNow,
            ShortLink = parent
        };

        _context.Visits.Add(visit);
        await _context.SaveChangesAsync();

        return visit;
    }
    
    public async Task<VisitsAnalyticsModel> GetAnalytics(DateTime startDate, DateTime endDate)
    {
        var query = _context.Visits
            .Where(visit => visit.Date >= startDate && visit.Date < endDate);
        
        var count = await query.CountAsync();

        var topFiveUrls = query
            .GroupBy(visit => visit.ShortLink)
            .OrderByDescending(group => group.Count())
            .Select(group => $"https://tpt.link/{group.Key.CustomAlias ?? group.Key.ShortCode!}")
            .Take(5)
            .AsEnumerable();

        return new VisitsAnalyticsModel(topFiveUrls, count);
    }
}