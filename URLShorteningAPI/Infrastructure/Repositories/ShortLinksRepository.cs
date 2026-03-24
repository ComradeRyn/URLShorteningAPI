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
    {
        throw new NotImplementedException();
    }

    public async Task<ShortLink?> GetByShortCode(string shortCode)
    {
        var accountId = _shortCodesService.Decode(shortCode);
        if (accountId == null)
        {
            return null;
        }

        return await _context.ShortLinks.FindAsync(accountId);
    }

    public async Task<ShortLink?> GetByCustomAlias(string customAlias)
    {
        var query = _context.ShortLinks as IQueryable<ShortLink>;

        return await query.FirstOrDefaultAsync(shortLink => shortLink.CustomAlias == customAlias);
    }

    public async Task<ShortLink> Add(string longUrl, string? customAlias, string? password)
    {
        var shortLink = new ShortLink
        {
            LongUrl = longUrl,
            CustomAlias = customAlias,
            Password = password,
            CreationDate = DateTime.Now
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