using System.Diagnostics.CodeAnalysis;
using Amazon;
using Amazon.Runtime;
using JasperFx.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.AmazonSqs;
using Wolverine.ErrorHandling;

namespace Infrastructure.Messaging;

public static class WolverineOptionsExtensions
{
    private const string DefaultRegion = "us-east-2";

    /// <summary>
    /// Shared Wolverine setup for the API and the Worker: AWS SQS transport with conventional,
    /// prefixed queue names and a retry policy for optimistic concurrency conflicts.
    /// </summary>
    [SuppressMessage("Globalization", "CA1308", Justification = "SQS queue names are case sensitive and the existing queues are lowercase.")]
    public static void ConfigureAuctionMessaging(this WolverineOptions options, IConfiguration configuration)
    {
        string prefix = configuration["AWS:PrefixQueue"] ?? string.Empty;

        options.UseAmazonSqsTransport(sqs =>
            {
                sqs.RegionEndpoint = RegionEndpoint.GetBySystemName(configuration["AWS:Region"] ?? DefaultRegion);

                sqs.DefaultAWSCredentials = new BasicAWSCredentials(
                    configuration["AWS:AccessKey"],
                    configuration["AWS:SecretKey"]);
            })
            .AutoProvision()
            .UseConventionalRouting(routing =>
            {
                routing.QueueNameForSender(type => $"{prefix}-{type.Name.ToLowerInvariant()}");
                routing.QueueNameForListener(type => $"{prefix}-{type.Name.ToLowerInvariant()}");
            });

        options.RestoreV5Defaults();

        options.DefaultLocalQueue.MaximumParallelMessages(1);

        options.Policies.OnException<DbUpdateConcurrencyException>()
            .RetryWithCooldown(
                100.Milliseconds(),
                100.Milliseconds(),
                100.Milliseconds(),
                100.Milliseconds(),
                100.Milliseconds());
    }
}
