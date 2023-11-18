using System.Security.Claims;

namespace MinApiOnNet.Endpoints.Custom;

public record UserNameRequest(ClaimsPrincipal ClaimsPrincipal) : EndpointRequest;

public class UserNameEndpoint : IEndpoint<UserNameRequest, IResult>
{
    public async Task<IResult> HandleAsync(UserNameRequest request, CancellationToken token)
    {
        return Results.Ok(new
        {
            UserName = request.ClaimsPrincipal?.Identity?.Name,
        });
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/username", async (ClaimsPrincipal principal)
                => await HandleAsync(new UserNameRequest(principal), CancellationToken.None))
            .Produces<string>()
            .RequireAuthorization(Constants.HasNameIdentifierPolicy)
            .WithTags("CustomEndpoints");
    }
}