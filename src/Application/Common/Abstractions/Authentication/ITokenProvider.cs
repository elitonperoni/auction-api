using System.Security.Claims;
using Domain.Entities;

namespace Application.Common.Abstractions.Authentication;

public interface ITokenProvider
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
