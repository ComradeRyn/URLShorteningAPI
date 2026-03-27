using System.Collections.ObjectModel;
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
    
    public async Task<int> GetCount(DateTime startDate, DateTime endDate)
        => await _context.ShortLinks
            .Where(link => link.CreationDate >= startDate && link.CreationDate < endDate)
            .CountAsync();

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
        => await _context.ShortLinks
            .FirstOrDefaultAsync(shortLink => shortLink.CustomAlias == customAlias);

    public async Task<ShortLink> Add(string longUrl, string? customAlias, string? password)
    {
        var shortLink = new ShortLink
        {
            LongUrl = longUrl,
            CustomAlias = customAlias,
            Password = password,
            CreationDate = DateTime.Now,
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
        DateTime startDate,
        DateTime endDate)
    {
        var query = _context.Entry(shortLink)
            .Collection(s => s.Visits)
            .Query()
            .Where(visit => visit.Date >= startDate && visit.Date < endDate);

        var totalVisits = await query.CountAsync();
        // Question: should this be null, or another value?
        DateTime? lastVisitedAt = totalVisits > 0 ? await query.MaxAsync(visit => visit.Date) : null;

        return new ShortLinkAnalyticsModel(totalVisits, lastVisitedAt);
    }
}