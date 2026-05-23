using System.Reflection;
using Application;
using AuctionApi;
using AuctionApi.Consumer;
using AuctionApi.Extensions;
using AuctionApi.Hubs;
using HealthChecks.UI.Client;
using Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Wolverine;
using Wolverine.RabbitMQ;

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

builder.Services.ConfigureRateLimiter();

builder.Host.UseWolverine(opts =>
{
    string host = builder.Configuration["RabbitMq:Host"] ?? "";
    string username = builder.Configuration["RabbitMq:Username"] ?? "";
    string password = builder.Configuration["RabbitMq:Password"] ?? "";

    opts.UseRabbitMq(rabbit =>
    {
        rabbit.HostName = host;
        rabbit.UserName = username;
        rabbit.Password = password;
    }).AutoProvision()              // cria exchanges/filas automaticamente
    .UseConventionalRouting();    // equivalente ao ConfigureEndpoints()
});

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services.AddSignalR_WithRedisBackplane(builder.Configuration);

string[] allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
    policy => policy
        .WithOrigins(allowedOrigins ?? [])
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials() 
));


WebApplication app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseSwaggerWithUi();

app.ApplyMigrations();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseCookiePolicy();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapEndpoints();

app.MapHub<AuctionHub>("/auctionHub");

await app.RunAsync();

// REMARK: Required for functional and integration tests to work.
namespace AuctionApi
{
    public partial class Program;
}
