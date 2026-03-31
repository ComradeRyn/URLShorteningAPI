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
        Expression<Func<ShortLink, bool>>? filterDateRange = (startDate, endDate) switch
        {
            (null, null) => null,
            (_, null) => visit => visit.CreationDate >= startDate,
            (null, _) => visit => visit.CreationDate < endDate,
            (_, _) => visit => visit.CreationDate >= startDate && visit.CreationDate < endDate
        };

        var query = _context.ShortLinks.AsQueryable();
        if (filterDateRange is not null)
        {
            query = query.Where(filterDateRange);
        }

        return await query.CountAsync();
    }

    public async Task<ShortLink?> GetByShortCode(string shortCode)
    {
        var accountId = _shortCodesService.Decode(shortCode);
        if (accountId is null)
        {
            return null;
        }

        return await _context.ShortLinks.FindAsync(accountId);
    }

    public async Task<ShortLink?> GetByCustomAlias(string customAlias)
        => await _context.ShortLinks.FirstOrDefaultAsync(shortLink => shortLink.CustomAlias == customAlias);

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
        Expression<Func<Visit, bool>>? filterDateRange = (startDate, endDate) switch
        {
            (null, null) => null,
            (_, null) => visit => visit.Date >= startDate,
            (null, _) => visit => visit.Date < endDate,
            (_, _) => visit => visit.Date >= startDate && visit.Date < endDate
        };

        var query = _context.Entry(shortLink)
            .Collection(s => s.Visits)
            .Query();

        if (filterDateRange is not null)
        {
            query = query.Where(filterDateRange);
        }
        
        var totalVisits = await query.CountAsync();
        var lastVisitedAt = totalVisits > 0 
            ? await query.MaxAsync(visit => visit.Date) 
            : (DateTime?)null;

        return new ShortLinkAnalyticsModel(totalVisits, lastVisitedAt);
    }
}