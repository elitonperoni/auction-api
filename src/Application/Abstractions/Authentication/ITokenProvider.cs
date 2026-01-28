using System.Security.Claims;
using Domain.Users;

namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
