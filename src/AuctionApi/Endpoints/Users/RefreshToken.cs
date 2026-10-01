using Application.Common.Abstractions.Messaging;
using Application.Features.Users.Commands.RefreshToken;
using AuctionApi.Extensions;
using AuctionApi.Infrastructure;
using SharedKernel;
using SharedKernel.Consts;

namespace AuctionApi.Endpoints.Users;

internal sealed class RefreshToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/refresh-token", async (
            HttpContext context,
            ICommandHandler<RefreshTokenCommand, RefreshTokenResponse> handler,
            CancellationToken cancellationToken) =>
        {
            string token = context.Request.Cookies[TokenConsts.AuthToken];
            string refreshToken = context.Request.Cookies[TokenConsts.RefreshToken];

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(token))
            {
                return Results.Unauthorized();
            }                

            var command = new RefreshTokenCommand(token, refreshToken);

            Result<RefreshTokenResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(
             response =>
             {
                 var cookieOptions = new CookieOptions
                 {
                     Expires = DateTimeOffset.UtcNow.AddDays(7),
                     Path = "/"
                 };

                 context.Response.Cookies.Append(TokenConsts.AuthToken, response.Token, cookieOptions);
                 context.Response.Cookies.Append(TokenConsts.RefreshToken, response.RefreshToken, cookieOptions);

                 return Results.Ok(new { message = "Tokens refreshed successfully" });
             },
             error => CustomResults.Problem(error)
         );
        }).WithTags(Tags.Users)
         .AllowAnonymous();
    }
}
