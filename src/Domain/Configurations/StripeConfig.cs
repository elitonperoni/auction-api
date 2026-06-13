namespace Domain.Configurations;

public sealed class StripeConfig
{
    public string BaseUrl { get; set; } = "https://api.stripe.com/v1/";
    public string SecretKey { get; set; } = string.Empty;
    public string SuccessUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
    public string Currency { get; set; } = "brl";
    public int ExpiresInMinutes { get; set; } = 60;
    public decimal SystemAccessAmount { get; set; }
    public string SystemAccessDescription { get; set; } = "Acesso ao sistema Auction";
}
