using System.Text;
using Amazon.S3;
using Application.Common.Abstractions.Authentication;
using Application.Common.Abstractions.Data;
using Application.Common.Abstractions.Mail;
using Application.Common.Interfaces;
using Application.Common.Options;
using Infrastructure.Authentication;
using Infrastructure.Caching;
using Infrastructure.Database;
using Infrastructure.DomainEvents;
using Infrastructure.ExternalServices;
using Infrastructure.Filters;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Consts;
using StackExchange.Redis;

namespace Infrastructure;

public static class DependencyInjection
{
    /// <summary>Services shared by every host (API and Worker).</summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddServices(configuration)
            .AddDatabase(configuration)
            .AddHealthChecks(configuration)
            .AddSecurityServices();

    /// <summary>JWT bearer authentication, cookie policy and authorization. Web hosts only.</summary>
    public static IServiceCollection AddWebAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment) =>
        services
            .AddJwtAuthentication(configuration, isDevelopment)
            .AddAuthorization();

    private static IServiceCollection AddServices(this IServiceCollection services,  IConfiguration configuration)
    {
        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        services.Configure<SecretsApi>(configuration.GetSection("SecretsApi"));        

        IConfigurationSection awsSection = configuration.GetSection("AWS");
        services.Configure<AwsConfig>(awsSection);
        services.Configure<StripeConfig>(configuration.GetSection("Stripe"));

        var credentials = new Amazon.Runtime.BasicAWSCredentials(
            awsSection["AccessKey"],
            awsSection["SecretKey"]
        );

        var region = Amazon.RegionEndpoint.GetBySystemName(awsSection["Region"] ?? "us-east-2");

        services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(credentials, region));

        services.AddScoped<IS3Service, S3Service>();
        services.AddScoped<IAuctionService, AuctionService>();
        services.AddScoped<ITelegramService, TelegramService>();
        services.AddScoped<IStripeService, StripeService>();
        services.AddSingleton<IMailSender, MailSender>();

        services.AddHttpClient(TelegramService.HttpClientName);
        services.AddHttpClient(StripeService.HttpClientName, (serviceProvider, client) =>
        {
            StripeConfig stripeConfig = serviceProvider
                .GetRequiredService<Microsoft.Extensions.Options.IOptions<StripeConfig>>()
                .Value;

            client.BaseAddress = new Uri(stripeConfig.BaseUrl);
        });

        return services;
    }
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        string? redisConnectionString = configuration.GetConnectionString("RedisConnection");

        if (string.IsNullOrEmpty(redisConnectionString))
        {
            throw new InvalidOperationException("The 'RedisConnection' connection string is not configured.");
        }
       
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString)
        );

        services.AddScoped<ICacheService, CacheService>();

        return services;
    }

    public static IServiceCollection AddSignalR_WithRedisBackplane(this IServiceCollection services, IConfiguration configuration)
    {
        string? redisConnectionString = configuration.GetConnectionString("RedisConnection");

        if (string.IsNullOrEmpty(redisConnectionString))
        {
            throw new InvalidOperationException("The 'RedisConnection' connection string is not configured.");
        }

        // Registered as singleton so the partitioned rate limiter state is shared across invocations
        services.AddSingleton<RateLimitingHubFilter>();
        services.AddSignalR(options => options.AddFilter<RateLimitingHubFilter>()).AddStackExchangeRedis(redisConnectionString);

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

    private static IServiceCollection AddSecurityServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();

        services.AddScoped<IUserContext, UserContext>();

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration, bool isDevelopment)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = !isDevelopment;
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

        return services;
    }
}
