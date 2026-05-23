
using Application;
using Auction.Worker.Consumer;
using Domain.Events;
using Infrastructure;
using JasperFx.CodeGeneration;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;

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
    string host = builder.Configuration["RabbitMq:Host"] ?? "";
    string username = builder.Configuration["RabbitMq:Username"] ?? "";
    string password = builder.Configuration["RabbitMq:Password"] ?? "";

    opts.UseRabbitMq(rabbit =>
    {
        rabbit.HostName = host;
        rabbit.UserName = username;
        rabbit.Password = password;
    })
        .AutoProvision()
        .UseConventionalRouting();

    opts.DefaultLocalQueue.MaximumParallelMessages(1);

    opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;

    opts.Policies.OnException<DbUpdateConcurrencyException>()
              .RetryWithCooldown(
                  TimeSpan.FromMilliseconds(100),
                  TimeSpan.FromMilliseconds(100),
                  TimeSpan.FromMilliseconds(100),
                  TimeSpan.FromMilliseconds(100),
                  TimeSpan.FromMilliseconds(100)
              );
});

builder.Services.AddRouting(); 
builder.Services.AddAuthorization();

IHost host = builder.Build();

await host.RunAsync();
