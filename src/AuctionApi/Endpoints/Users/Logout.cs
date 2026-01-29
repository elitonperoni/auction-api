namespace AuctionApi.Endpoints.Users;

internal sealed class Logout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/logout", async (HttpContext context, CancellationToken cancellationToken) =>
        {
            var cookieOptions = new CookieOptions
            {            
                Expires = DateTimeOffset.UtcNow.AddDays(-1), 
                Path = "/"
            };

            context.Response.Cookies.Append("auth-token", "", cookieOptions);
            context.Response.Cookies.Append("refresh-token", "", cookieOptions);

            return Results.NoContent();
        })
        .WithTags(Tags.Users);
    }
}
