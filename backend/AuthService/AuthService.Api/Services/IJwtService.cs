using System.Security.Claims;
using AuthService.Api.Models;

namespace AuthService.Api.Services
{
    public interface IJwtService
    {
        string GenerateLoginJwt(User user);
        string GenerateEmailConfirmationJwt(User user);
        ClaimsPrincipal? ValidateEmailConfirmationJwt(string token);
    }
}