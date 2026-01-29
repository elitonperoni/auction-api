using System.Text;
using Amazon.S3;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Mail;
using Application.Common.Interfaces;
using Application.Mail;
using Domain.Configurations;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
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

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddServices(configuration)
            .AddDatabase(configuration)
            .AddHealthChecks(configuration)
            .AddAuthenticationInternal(configuration)
            .AddAuthorizationInternal();

    private static IServiceCollection AddServices(this IServiceCollection services,  IConfiguration configuration)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        services.Configure<SecretsApi>(configuration.GetSection("SecretsApi"));        

        IConfigurationSection awsSection = configuration.GetSection("AWS");
        services.Configure<AwsConfig>(awsSection);

        //services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        //services.AddAWSService<IAmazonS3>();


        ////// 2. Cria as credenciais manualmente com os dados do JSON
        var credentials = new Amazon.Runtime.BasicAWSCredentials(
            awsSection["AccessKey"],
            awsSection["SecretKey"]
        );

        //// 3. Define a região
        var region = Amazon.RegionEndpoint.GetBySystemName(awsSection["Region"] ?? "us-east-2");

        //// 4. Registra o IAmazonS3 forçando o uso dessas credenciais
        services.AddSingleton<IAmazonS3>(sp =>
            new AmazonS3Client(credentials, region)
        );

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
        IConfiguration configuration)
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
                        string accessToken = context.Request.Query["access_token"];
                        context.Token = !string.IsNullOrEmpty(accessToken) &&
                            context.HttpContext.Request.Path.StartsWithSegments("/auctionHub")
                            ? accessToken
                            : context.Request.Cookies["auth-token"];
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
            };
        });

        services.AddHttpContextAccessor();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<IMailSender, MailSender>();

        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IS3Service, S3Service>();  

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
