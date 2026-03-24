using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UrlShorteningContext (DbContextOptions<UrlShorteningContext> options) : DbContext(options)
{
    public DbSet<ShortLink> ShortLinks { get; init; }
    public DbSet<Visit> Visits { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ShortLink>()
            .Property(x => x.Id)
            .UseIdentityColumn(seed: 121, increment: 3);
    }
}