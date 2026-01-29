using Application.Abstractions.Messaging;
using Application.Users.Login;
using Application.Users.RefreshToken;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;

namespace AuctionApi.Endpoints.Users;

public class RefreshToken : IEndpoint
{
    public sealed record Request(string Token, string RefreshToken);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/refresh-token", async (
            HttpContext context,
            ICommandHandler<RefreshTokenCommand, RefreshTokenResponse> handler,
            CancellationToken cancellationToken) =>
        {
            string token = context.Request.Cookies["auth-token"];
            string refreshToken = context.Request.Cookies["refresh-token"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Results.Unauthorized();
            }                

            var command = new RefreshTokenCommand(token ?? "", refreshToken);

            Result<RefreshTokenResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(
             response =>
             {
                 var cookieOptions = new CookieOptions
                 {
                     Expires = DateTimeOffset.UtcNow.AddDays(7),
                     Path = "/"
                 };

                 context.Response.Cookies.Append("auth-token", response.Token, cookieOptions);
                 context.Response.Cookies.Append("refresh-token", response.RefreshToken, cookieOptions);

                 return Results.Ok(new { message = "Tokens atualizados com sucesso" });
             },
             error => CustomResults.Problem(error)
         );
        }).WithTags(Tags.Users)
         .AllowAnonymous();
    }
}
