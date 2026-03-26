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
    
    public async Task<VisitsAnalyticsModel> GetAnalytics(DateTime startDate, DateTime endDate)
    {
        var query = _context.Visits as IQueryable<Visit>;
        query = query.Where(visit => visit.Date >= startDate && visit.Date <= endDate);
        
        var count = await query.CountAsync();

        var topFiveUrls = query
            .GroupBy(visit => visit.ShortLink)
            .OrderBy(group => group.Count())
            .Select(group => group.Key.LongUrl)
            .AsEnumerable();

        return new VisitsAnalyticsModel(topFiveUrls, count);
    }
}