using System.Collections.ObjectModel;
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
            .Where(link => link.CreationDate >= startDate && link.CreationDate <= endDate)
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
}