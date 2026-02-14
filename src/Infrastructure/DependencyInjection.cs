using System.Text;
using Amazon.S3;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Mail;
using Application.AuctionUseCases.Services;
using Application.Interfaces;
using Application.Mail;
using Domain.Configurations;
using Domain.Interfaces;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Caching;
using Infrastructure.Database;
using Infrastructure.DomainEvents;
using Infrastructure.ExternalServices;
using Infrastructure.Time;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel;
using SharedKernel.Consts;
using StackExchange.Redis;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment) =>
        services
            .AddServices(configuration)
            .AddDatabase(configuration)
            .AddHealthChecks(configuration)
            .AddAuthenticationInternal(configuration, isDevelopment)
            .AddAuthorizationInternal();

    private static IServiceCollection AddServices(this IServiceCollection services,  IConfiguration configuration)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        services.Configure<SecretsApi>(configuration.GetSection("SecretsApi"));        

        IConfigurationSection awsSection = configuration.GetSection("AWS");
        services.Configure<AwsConfig>(awsSection);
        var credentials = new Amazon.Runtime.BasicAWSCredentials(
            awsSection["AccessKey"],
            awsSection["SecretKey"]
        );

        var region = Amazon.RegionEndpoint.GetBySystemName(awsSection["Region"] ?? "us-east-2");

        services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(credentials, region));

        services.AddScoped<IS3Service, S3Service>();
        services.AddScoped<IAuctionService, AuctionService>();

        return services;
    }
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        string? redisConnectionString = configuration.GetConnectionString("RedisConnection");

        if (string.IsNullOrEmpty(redisConnectionString))
        {
            throw new Exception("ALERTA: A Connection String 'RedisConnection' não foi encontrada no appsettings.json!");
        }
       
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString)
        );

        services.AddScoped<INotificationCacheService, NotificationCacheService>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Database")!);

        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services,
        IConfiguration configuration, bool isDevelopment)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        string accessToken = context.Request.Cookies[TokenConsts.AuthToken];

                        context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.None;             
            options.OnAppendCookie = cookieContext =>
            {
                cookieContext.CookieOptions.Secure = true;
                cookieContext.CookieOptions.HttpOnly = true;
                cookieContext.CookieOptions.SameSite = SameSiteMode.None;

                if (!isDevelopment)
                {
                    cookieContext.CookieOptions.Domain = configuration["SecretsApi:Domain"]; 
                }
            };
        });

        services.AddHttpContextAccessor();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<IMailSender, MailSender>();

        services.AddScoped<IUserContext, UserContext>();        

        return services;
    }

    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
        {
        services.AddAuthorization();

        services.AddScoped<PermissionProvider>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }
}
