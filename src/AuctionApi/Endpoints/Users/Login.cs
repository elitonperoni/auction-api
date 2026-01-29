using Application.Abstractions.Messaging;
using Application.Users.Login;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace AuctionApi.Endpoints.Users;

internal sealed class Login : IEndpoint
{
    public sealed record Request(string Email, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/login", async (
            Request request,
            ICommandHandler<LoginUserCommand, LoginResponse> handler,
            HttpContext context, 
            CancellationToken cancellationToken) =>
        {
            var command = new LoginUserCommand(request.Email, request.Password);

            Result<LoginResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                response => 
                {
                    var cookieOptions = new CookieOptions
                    {  
                        Path = "/",
                        Expires = DateTime.UtcNow.AddDays(7)
                    };

                    context.Response.Cookies.Append("auth-token", response.Token, cookieOptions);
                    context.Response.Cookies.Append("refresh-token", response.RefreshToken, cookieOptions);

                    return Results.Ok(new { response.Id, response.Name });
                },
                error => CustomResults.Problem(error)
            );
        })
        .WithTags(Tags.Users);
    }
}
