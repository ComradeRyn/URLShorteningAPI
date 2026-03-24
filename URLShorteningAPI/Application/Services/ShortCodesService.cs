using Application.Interfaces;
using Sqids;

namespace Application.Services;

public class ShortCodesService : IShortCodesService
{
    private readonly SqidsEncoder<long> _sqids;

    public ShortCodesService(SqidsEncoder<long> sqids)
    {
        _sqids = sqids;
    }

    public string Encode(long id)
        => _sqids.Encode(id);

    public long Decode(string shortCode)
        => _sqids.Decode(shortCode)[0];
}