using Application;
using Auction.Worker.Infrastructure;
using Infrastructure;
using Wolverine;

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
