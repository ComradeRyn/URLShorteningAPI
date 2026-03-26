namespace Domain.Models;

public class ShortLink
{
    public long Id { get; init; }
    public required string LongUrl { get; init; }
    public string? ShortCode { get; set; }
    public string? CustomAlias { get; init; }
    public string? Password { get; init; }
    public DateTime CreationDate { get; init; }
    public required ICollection<Visit> Visits { get; init; }
}