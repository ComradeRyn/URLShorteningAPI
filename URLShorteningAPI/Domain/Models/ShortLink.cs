namespace Domain.Models;

public class ShortLink
{
    public long Id;
    public required string LongUrl;
    public string? ShortCode;
    public string? CustomAlias;
    public string? Password;
    public DateTime CreationDate;
    public required ICollection<Visit> Visits;
}