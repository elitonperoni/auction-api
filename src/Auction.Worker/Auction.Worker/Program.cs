
using Application;
using Auction.Worker.Consumer;
using Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BidPlacedConsumer>(cfg => cfg.UseMessageRetry(r =>
        {
            r.Handle<DbUpdateConcurrencyException>();            
            r.Interval(5, TimeSpan.FromMilliseconds(100));
        }));    

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "", "/", h => {
            h.Username(builder.Configuration["RabbitMq:Username"] ?? "");
            h.Password(builder.Configuration["RabbitMq:Password"] ?? "");
        });
        cfg.PrefetchCount = 1;
        cfg.ConfigureEndpoints(context);

    });
});

builder.Services.AddRouting(); 
builder.Services.AddAuthorization();

IHost host = builder.Build();

await host.RunAsync();
