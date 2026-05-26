
using Amazon;
using Amazon.Runtime;
using Application;
using Infrastructure;
using JasperFx.Core;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.AmazonSqs;
using Wolverine.ErrorHandling;
using Auction.Worker.Infrastructure;

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

builder.AddWolverine(builder.Configuration);

builder.Services.AddRouting(); 
builder.Services.AddAuthorization();

IHost host = builder.Build();

await host.RunAsync();
