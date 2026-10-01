using Infrastructure.Messaging;
using Wolverine;

namespace AuctionApi.Infrastructure;

public static class WolverineConfiguration
{
    public static void AddWolverine(this WebApplicationBuilder builder, IConfiguration configuration) =>
        builder.UseWolverine(options => options.ConfigureAuctionMessaging(configuration));
}
