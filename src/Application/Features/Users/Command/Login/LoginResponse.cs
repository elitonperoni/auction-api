namespace Application.Features.Users.Command.Login;

public sealed record LoginResponse
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string Name { get; set; }
}
