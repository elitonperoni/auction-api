
using Application;
using Application.Common.Abstractions.Messaging;
using Auction.Worker.Consumer;
using Domain.Events;
using Infrastructure;
using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Model;
using JasperFx.Core;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;
using Wolverine.Runtime;
using Wolverine.Runtime.Handlers;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Configuration
    .AddEnvironmentVariables();

builder.Services
    .AddApplication()
    .AddInfrastructure(
        builder.Configuration,
        builder.Environment.IsDevelopment());

builder.Services.AddCaching(builder.Configuration);

builder.UseWolverine(opts =>
{
    opts.UseRabbitMq(rabbit =>
    {
        rabbit.HostName = builder.Configuration["RabbitMq:Host"] ?? "";
        rabbit.UserName = builder.Configuration["RabbitMq:Username"] ?? "";
        rabbit.Password = builder.Configuration["RabbitMq:Password"] ?? "";
    }).AutoProvision()
     .UseConventionalRouting();

   // opts.UseRuntimeCompilation();

    opts.RestoreV5Defaults();

    opts.DefaultLocalQueue.MaximumParallelMessages(1);

    //opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;

    opts.Policies.OnException<DbUpdateConcurrencyException>()
        .RetryWithCooldown(
            100.Milliseconds(),
            100.Milliseconds(),
            100.Milliseconds(),
            100.Milliseconds(),
            100.Milliseconds());
});

builder.Services.AddRouting(); 
builder.Services.AddAuthorization();

IHost host = builder.Build();

await host.RunAsync();
