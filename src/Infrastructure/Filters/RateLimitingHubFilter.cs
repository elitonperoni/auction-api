using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace Infrastructure.Filters;

public class RateLimitingHubFilter : IHubFilter
{
    private readonly PartitionedRateLimiter<HubCallerContext> _rateLimiter;

    public RateLimitingHubFilter()
    {
        _rateLimiter = PartitionedRateLimiter.Create<HubCallerContext, string>(context =>
        {
            string partitionKey = context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                               ?? context.ConnectionId;

            return RateLimitPartition.GetFixedWindowLimiter(partitionKey,
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromSeconds(1),
                    AutoReplenishment = true
                });
        });
    }

    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        if (invocationContext.HubMethodName.Equals(ChannelNames.SendBid, StringComparison.OrdinalIgnoreCase))
        {
            using RateLimitLease lease = _rateLimiter.AttemptAcquire(invocationContext.Context);

            if (!lease.IsAcquired)
            {
                throw new HubException("RateLimitExceeded: Too many requests. Please try again later.");
            }
        }

        return await next(invocationContext);
    }
}
