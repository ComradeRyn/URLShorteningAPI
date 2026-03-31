namespace Domain.Models;

public class Visit
{
    public long Id { get; init; }
    public DateTime Date { get; init; }
    public required long ShortLinkId { get; init; }
}