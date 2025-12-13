using System.Security.Cryptography;

namespace AuctionApi.Extensions;

public static class TokenGenerator
{
    public static string GenerateSecureToken(int size = 32)
    {
        byte[] randomBytes = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        return Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
