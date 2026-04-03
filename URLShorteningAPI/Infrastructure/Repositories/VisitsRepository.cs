using System.Linq.Expressions;
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
    
    public async Task<Visit> Add(long parentId)
    {
        var visit = new Visit
        {
            Date = DateTime.UtcNow,
            ShortLinkId = parentId
        };

        _context.Visits.Add(visit);
        await _context.SaveChangesAsync();

        return visit;
    }
    
    public async Task<VisitsAnalyticsModel> GetAnalytics(DateTime? startDate, DateTime? endDate)
    {
        Expression<Func<Visit, bool>> dateRangeFilter = (startDate, endDate) switch
        {
            (null, null) => visit => true,
            (_, null) => visit => visit.Date >= startDate,
            (null, _) => visit => visit.Date < endDate,
            (_, _) => visit => visit.Date >= startDate && visit.Date < endDate
        };
        
        var visitsQuery = _context.Visits
            .AsQueryable()
            .Where(dateRangeFilter);
        
        var count = await visitsQuery.CountAsync();

        var topFiveIds = visitsQuery
            .GroupBy(visit => visit.ShortLinkId)
            .OrderByDescending(group => group.Count())
            .Take(5)
            .Select(group => group.Key);
        
        var topFiveUrls =  _context.ShortLinks
            .Where(shortLink => topFiveIds.Contains(shortLink.Id))
            .Select(shortLink => $"https://tpt.link/{shortLink.CustomAlias ?? shortLink.ShortCode}")
            .AsEnumerable();

        return new VisitsAnalyticsModel(topFiveUrls, count);
    }
}