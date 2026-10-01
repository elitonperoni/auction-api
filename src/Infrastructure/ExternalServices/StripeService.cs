using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using Application.Common.Interfaces;
using Application.Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Infrastructure.ExternalServices;

internal sealed partial class StripeService(
    IHttpClientFactory httpClientFactory,
    IOptions<StripeConfig> options,
    ILogger<StripeService> logger) : IStripeService
{
    public const string HttpClientName = "stripe";

    private readonly StripeConfig _config = options.Value;

    public async Task<Result<StripeCheckoutResponse>> CreateSystemAccessCheckoutLinkAsync(
        StripeSystemAccessCheckoutRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_config.SecretKey))
        {
            return Result.Failure<StripeCheckoutResponse>(
                Error.Failure("Stripe.NotConfigured", "Stripe secret key is not configured"));
        }

        if (_config.SystemAccessAmount <= 0)
        {
            return Result.Failure<StripeCheckoutResponse>(
                Error.Failure("Stripe.InvalidSystemAccessAmount", "System access amount is not configured"));
        }

        if (string.IsNullOrWhiteSpace(_config.SuccessUrl))
        {
            return Result.Failure<StripeCheckoutResponse>(
                Error.Failure("Stripe.SuccessUrlNotConfigured", "Stripe success URL is not configured"));
        }

        try
        {
            return await CreateCheckoutSessionAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            LogRequestException(logger, ex, request.UserId);
            return Result.Failure<StripeCheckoutResponse>(CheckoutUnavailable);
        }
        catch (JsonException ex)
        {
            LogInvalidResponse(logger, ex, request.UserId);
            return Result.Failure<StripeCheckoutResponse>(CheckoutUnavailable);
        }
    }

    private static Error CheckoutUnavailable => Error.Failure(
        "Stripe.CreateCheckoutSessionFailed",
        "Could not create the checkout session. Please try again later.");

    private async Task<Result<StripeCheckoutResponse>> CreateCheckoutSessionAsync(
        StripeSystemAccessCheckoutRequest request,
        CancellationToken cancellationToken)
    {
        using HttpClient httpClient = httpClientFactory.CreateClient(HttpClientName);

        using var message = new HttpRequestMessage(HttpMethod.Post, "checkout/sessions")
        {
            Content = new FormUrlEncodedContent(CreateCheckoutSessionPayload(request))
        };

        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config.SecretKey);
        // Retrying a registration for the same user never creates a second checkout session.
        message.Headers.Add("Idempotency-Key", $"system-access-{request.UserId:N}");

        using HttpResponseMessage response = await httpClient.SendAsync(message, cancellationToken);

        string content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            // Stripe's error body stays in the logs; clients only get a generic message.
            LogCheckoutFailed(logger, request.UserId, (int)response.StatusCode, content);
            return Result.Failure<StripeCheckoutResponse>(CheckoutUnavailable);
        }

        StripeCheckoutResponse? checkoutResponse = ParseCheckoutResponse(content);

        if (checkoutResponse is null || string.IsNullOrWhiteSpace(checkoutResponse.CheckoutUrl))
        {
            return Result.Failure<StripeCheckoutResponse>(
                Error.Failure("Stripe.CheckoutUrlNotFound", "Stripe did not return the checkout URL"));
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

    [LoggerMessage(Level = LogLevel.Error, Message = "Stripe checkout session creation failed for user {UserId} with status {StatusCode}: {Body}")]
    private static partial void LogCheckoutFailed(ILogger logger, Guid userId, int statusCode, string body);

    [LoggerMessage(Level = LogLevel.Error, Message = "Stripe request failed for user {UserId}")]
    private static partial void LogRequestException(ILogger logger, Exception exception, Guid userId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Stripe returned an unreadable response for user {UserId}")]
    private static partial void LogInvalidResponse(ILogger logger, Exception exception, Guid userId);
}
