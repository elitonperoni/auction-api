using System.Globalization;
using Amazon;
using Amazon.Runtime;
using JasperFx.Core;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.AmazonSqs;
using Wolverine.ErrorHandling;

namespace AuctionApi.Infrastructure;

public static class WolverineConfiguration
{
    public static void AddWolverine(
        this WebApplicationBuilder builder,
        IConfiguration configuration)
    {
        string prefix = configuration["AWS:PrefixQueue"];
        builder.UseWolverine(opts =>
        {
            opts.UseAmazonSqsTransport(sqs =>
            {
                sqs.RegionEndpoint = RegionEndpoint.GetBySystemName(
                    configuration["AWS:Region"] ?? "us-east-2");

                sqs.DefaultAWSCredentials = new BasicAWSCredentials(
                    configuration["AWS:AccessKey"],
                    configuration["AWS:SecretKey"]);
            })
            .AutoProvision()
            .UseConventionalRouting(routing =>
            {
                routing.QueueNameForSender(t => $"{prefix}-{t.Name.ToLower(CultureInfo.CurrentCulture)}");
                routing.QueueNameForListener(t => $"{prefix}-{t.Name.ToLower(CultureInfo.CurrentCulture)}");
            });

            opts.RestoreV5Defaults();

            opts.DefaultLocalQueue.MaximumParallelMessages(1);

            opts.Policies.OnException<DbUpdateConcurrencyException>()
                .RetryWithCooldown(
                    100.Milliseconds(),
                    100.Milliseconds(),
                    100.Milliseconds(),
                    100.Milliseconds(),
                    100.Milliseconds());
        });
    }
}
