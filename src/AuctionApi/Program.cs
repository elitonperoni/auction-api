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
    .AddInfrastructure(builder.Configuration);


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BidProcessedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
}
    );

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services.AddSignalR();

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
