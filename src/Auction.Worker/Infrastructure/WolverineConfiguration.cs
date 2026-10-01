using Infrastructure.Messaging;
using Wolverine;

namespace Auction.Worker.Infrastructure;

public static class WolverineConfiguration
{
    public static void AddWolverine(this HostApplicationBuilder builder, IConfiguration configuration) =>
        builder.UseWolverine(options => options.ConfigureAuctionMessaging(configuration));
}
