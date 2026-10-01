using System.Reflection;
using Application;
using AuctionApi;
using AuctionApi.Extensions;
using AuctionApi.Hubs;
using AuctionApi.Infrastructure;
using HealthChecks.UI.Client;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Configuration
    .AddEnvironmentVariables();

builder.Services.AddSwaggerGenWithAuth();

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(
        builder.Configuration, 
        builder.Environment.IsDevelopment());

builder.Services.AddCaching(builder.Configuration);

builder.AddWolverine(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services.AddSignalR_WithRedisBackplane(builder.Configuration);

const string corsPolicyName = "CorsPolicy";

string[] allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options => options.AddPolicy(corsPolicyName,
    policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

WebApplication app = builder.Build();

app.UseRequestContextLogging();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(corsPolicyName);

app.UseSwaggerWithUi();

if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseCookiePolicy();

app.UseAuthentication();

app.UseAuthorization();

app.MapEndpoints();

app.MapHub<AuctionHub>(AuctionHub.Route);

await app.RunAsync();

namespace AuctionApi
{
    public partial class Program;
}
