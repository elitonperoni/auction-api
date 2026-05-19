namespace Application.Features.Users.Command.RefreshToken;

public sealed record RefreshTokenResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}
