namespace Domain.Models;

public class Visit
{
    public long Id { get; init; }
    public DateTime Date { get; init; }
    public required ShortLink ShortLink { get; init; }
}