using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        
        /// <summary>
        /// Creates a JWT
        /// </summary>
        /// <returns>A JWT allowing access to Accounts controller</returns>
        [HttpGet]
        public ActionResult<TokenResponse> GetAuthenticationToken()
            => Ok(_authenticationService.CreateToken());
    }
}