using Application.DTOs.Responses;

namespace Application.Interfaces;

public interface IAuthenticationService
{
    TokenResponse CreateToken();
}