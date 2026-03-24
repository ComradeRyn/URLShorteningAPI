namespace Application.DTOs.Requests;

public record VerifyPasswordRequest(string ShortAlias, string Password);