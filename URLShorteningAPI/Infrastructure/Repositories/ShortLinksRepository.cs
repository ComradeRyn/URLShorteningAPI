using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ShortLinksRepository : IShortLinksRepository
{
    private readonly UrlShorteningContext _context;
    private readonly IShortCodesService _shortCodesService;

    public ShortLinksRepository(UrlShorteningContext context, IShortCodesService shortCodesService)
    {
        _context = context;
        _shortCodesService = shortCodesService;
    }

    public async Task<int> GetCount(DateTime? startDate, DateTime? endDate)
    {
        Expression<Func<ShortLink, bool>> filterDateRange = (startDate, endDate) switch
        {
            (null, null) => shortLink => true,
            (_, null) => shortLink => shortLink.CreationDate >= startDate,
            (null, _) => shortLink => shortLink.CreationDate < endDate,
            (_, _) => shortLink => shortLink.CreationDate >= startDate && shortLink.CreationDate < endDate
        };

        return await _context.ShortLinks
            .Where(filterDateRange)
            .CountAsync();
    }

    public async Task<ShortLink?> Get(string shortAlias)
    {
        var accountId = _shortCodesService.Decode(shortAlias);
        if (accountId is not null)
        {
            return await _context.ShortLinks.FindAsync(accountId);
        }

        return await _context.ShortLinks.FirstOrDefaultAsync(shortLink => shortLink.CustomAlias == shortAlias);
    }

    public async Task<ShortLink> Add(string longUrl, string? customAlias, string? password)
    {
        var shortLink = new ShortLink
        {
            LongUrl = longUrl,
            CustomAlias = customAlias,
            Password = password,
            CreationDate = DateTime.UtcNow,
            Visits = new Collection<Visit>()
        };

        _context.ShortLinks.Add(shortLink);
        await _context.SaveChangesAsync();

        if (customAlias is null)
        {
            shortLink.ShortCode = _shortCodesService.Encode(shortLink.Id);
            await _context.SaveChangesAsync();
        }

        return shortLink;
    }

    public async Task<ShortLinkAnalyticsModel> GetAnalytics(
        ShortLink shortLink,
        DateTime? startDate,
        DateTime? endDate)
    {
        Expression<Func<Visit, bool>> filterDateRange = (startDate, endDate) switch
        {
            (null, null) => visit => true,
            (_, null) => visit => visit.Date >= startDate,
            (null, _) => visit => visit.Date < endDate,
            (_, _) => visit => visit.Date >= startDate && visit.Date < endDate
        };

        var query = _context.Entry(shortLink)
            .Collection(s => s.Visits)
            .Query()
            .Where(filterDateRange);
        
        var totalVisits = await query.CountAsync();
        var lastVisitedAt = totalVisits > 0 
            ? await query.MaxAsync(visit => visit.Date) 
            : (DateTime?)null;

        return new ShortLinkAnalyticsModel(totalVisits, lastVisitedAt);
    }
}