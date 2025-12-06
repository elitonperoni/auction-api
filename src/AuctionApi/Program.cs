using System.Reflection;
using Application;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using AuctionApi;
using AuctionApi.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Configuration
    .AddEnvironmentVariables();

builder.Services.AddSwaggerGenWithAuth();

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);


builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services.AddSignalR();

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
    policy => policy
        .WithOrigins("http://localhost:3000", 
         "null",
         "https://main.d2x71qee6ioduv.amplifyapp.com/")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials() 
));

WebApplication app = builder.Build();

app.UseCors("CorsPolicy");

app.MapEndpoints();

app.UseSwaggerWithUi();

if (app.Environment.IsDevelopment())
{    
    app.ApplyMigrations();
}

//app.MapHealthChecks("health", new HealthCheckOptions
//{
//    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//});

//app.UseRequestContextLogging();

//app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<AuctionHub>("/auctionHub");

await app.RunAsync();

// REMARK: Required for functional and integration tests to work.
namespace AuctionApi
{
    public partial class Program;
}
