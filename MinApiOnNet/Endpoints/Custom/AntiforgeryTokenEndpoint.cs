using Microsoft.AspNetCore.Antiforgery;

namespace MinApiOnNet.Endpoints.Custom;

public record AntiforgeryTokenRequest(HttpContext HttpContext) : EndpointRequest;

public class AntiforgeryTokenEndpoint : IEndpoint<AntiforgeryTokenRequest, IResult>
{
    private readonly IAntiforgery _antiforgery;

    public AntiforgeryTokenEndpoint(IAntiforgery antiforgery) => _antiforgery = antiforgery;

    public async Task<IResult> HandleAsync(AntiforgeryTokenRequest request, CancellationToken token)
    {
        var tokens = _antiforgery.GetAndStoreTokens(request.HttpContext);
        request.HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, new CookieOptions {HttpOnly = false});

        return Results.Ok();
    }

    public void Map(IEndpointRouteBuilder app)
    {
        // Endpoint for CSRF token retrieval. For more details see:
        // https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-6.0#javascript-1
        app
            .MapGet("antiforgery/token", async (HttpContext context, CancellationToken token)
                => await HandleAsync(new AntiforgeryTokenRequest(context), token))
            .WithTags("CustomEndpoints");
    }
}