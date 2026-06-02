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
            .UseConventionalRouting();

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
