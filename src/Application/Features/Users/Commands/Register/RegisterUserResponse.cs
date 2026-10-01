namespace Application.Features.Users.Commands.Register;

public sealed class RegisterUserResponse
{
    public Guid UserId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string CheckoutUrl { get; set; } = string.Empty;
}
