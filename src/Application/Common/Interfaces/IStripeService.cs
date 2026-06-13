using SharedKernel;

namespace Application.Common.Interfaces;

public interface IStripeService
{
    Task<Result<StripeCheckoutResponse>> CreateSystemAccessCheckoutLinkAsync(
        StripeSystemAccessCheckoutRequest request,
        CancellationToken cancellationToken);
}

public sealed record StripeSystemAccessCheckoutRequest(
    Guid UserId,
    string CustomerName,
    string CustomerEmail);

public sealed class StripeCheckoutResponse
{
    public string OrderId { get; set; } = string.Empty;
    public string CheckoutUrl { get; set; } = string.Empty;
}
