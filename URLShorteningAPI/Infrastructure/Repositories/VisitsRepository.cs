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

    // TODO: come up with a better name
    public async Task<int> GetFromShortLinkCount(ShortLink shortLink)
         => await _context.Entry(shortLink)
            .Collection(s => s.Visits)
            .Query()
            .CountAsync();

    public async Task<Visit> Add(ShortLink parent)
    {
        var visit = new Visit
        {
            Date = DateTime.Now,
            ShortLink = parent
        };

        _context.Visits.Add(visit);
        await _context.SaveChangesAsync();

        return visit;
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