using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using Application.Common.Interfaces;
using Domain.Configurations;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Infrastructure.ExternalServices;

public sealed class StripeService(
    IHttpClientFactory httpClientFactory,
    IOptions<StripeConfig> options) : IStripeService
{
    private readonly StripeConfig _config = options.Value;

    public async Task<Result<StripeCheckoutResponse>> CreateSystemAccessCheckoutLinkAsync(
        StripeSystemAccessCheckoutRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_config.SecretKey))
        {
            return Result.Failure<StripeCheckoutResponse>(
                Error.Failure("Stripe.NotConfigured", "Chave secreta da Stripe não configurada"));
        }

        if (_config.SystemAccessAmount <= 0)
        {
            return Result.Failure<StripeCheckoutResponse>(
                Error.Failure("Stripe.InvalidSystemAccessAmount", "Valor do acesso ao sistema não configurado"));
        }

        if (string.IsNullOrWhiteSpace(_config.SuccessUrl))
        {
            return Result.Failure<StripeCheckoutResponse>(
                Error.Failure("Stripe.SuccessUrlNotConfigured", "URL de sucesso da Stripe não configurada"));
        }

        using HttpClient httpClient = httpClientFactory.CreateClient("stripe");
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _config.SecretKey);

        using var formContent = new FormUrlEncodedContent(CreateCheckoutSessionPayload(request));

        HttpResponseMessage response = await httpClient.PostAsync(
            "checkout/sessions",
            formContent,
            cancellationToken);

        string content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure<StripeCheckoutResponse>(
                Error.Failure("Stripe.CreateCheckoutSessionFailed", content));
        }

        StripeCheckoutResponse? checkoutResponse = ParseCheckoutResponse(content);

        if (checkoutResponse is null || string.IsNullOrWhiteSpace(checkoutResponse.CheckoutUrl))
        {
            return Result.Failure<StripeCheckoutResponse>(
                Error.Failure("Stripe.CheckoutUrlNotFound", "Stripe não retornou a URL do checkout"));
        }

        return Result.Success(checkoutResponse);
    }

    private List<KeyValuePair<string, string>> CreateCheckoutSessionPayload(
        StripeSystemAccessCheckoutRequest request)
    {
        var payload = new List<KeyValuePair<string, string>>
        {
            new("mode", "payment"),
            new("success_url", _config.SuccessUrl),
            new("client_reference_id", request.UserId.ToString("N")),
            new("customer_email", request.CustomerEmail),
            new("line_items[0][quantity]", "1"),
            new("line_items[0][price_data][currency]", _config.Currency),
            new("line_items[0][price_data][unit_amount]", ToCents(_config.SystemAccessAmount).ToString(CultureInfo.InvariantCulture)),
            new("line_items[0][price_data][product_data][name]", _config.SystemAccessDescription),
            new("metadata[user_id]", request.UserId.ToString()),
            new("metadata[customer_name]", request.CustomerName)
        };

        if (!string.IsNullOrWhiteSpace(_config.CancelUrl))
        {
            payload.Add(new KeyValuePair<string, string>("cancel_url", _config.CancelUrl));
        }

        long expiresAt = DateTimeOffset.UtcNow
            .AddMinutes(Math.Clamp(_config.ExpiresInMinutes, 30, 1440))
            .ToUnixTimeSeconds();

        payload.Add(new KeyValuePair<string, string>("expires_at", expiresAt.ToString(CultureInfo.InvariantCulture)));

        return payload;
    }

    private static int ToCents(decimal amount)
    {
        return decimal.ToInt32(decimal.Round(amount * 100, 0, MidpointRounding.AwayFromZero));
    }

    private static StripeCheckoutResponse? ParseCheckoutResponse(string content)
    {
        using var document = JsonDocument.Parse(content);
        JsonElement root = document.RootElement;

        string orderId = GetString(root, "id") ?? string.Empty;
        string? checkoutUrl = GetString(root, "url");

        return checkoutUrl is null
            ? null
            : new StripeCheckoutResponse
            {
                OrderId = orderId,
                CheckoutUrl = checkoutUrl
            };
    }

    private static string? GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out JsonElement value) &&
               value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }
}
