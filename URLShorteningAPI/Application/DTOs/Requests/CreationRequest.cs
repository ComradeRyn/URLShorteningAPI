namespace Application.DTOs.Requests;

public record CreationRequest(
    string LongUrl,
    string? CustomAlias,
    string? Password);