namespace Application.Interfaces;

public interface IShortCodesService
{
    string Encode(long id);
    long Decode(string shortCode);
}