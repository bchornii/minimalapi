namespace MinApiOnNet.Endpoints.Custom;

public record UrlNormalizeRequest(string Url) : EndpointRequest;

public class UrlNormalizeEndpoint : IEndpoint<UrlNormalizeRequest, IResult>
{
    public async Task<IResult> HandleAsync(UrlNormalizeRequest request, CancellationToken token)
    {
        var result = request.Url.NormalizeUrl();
        return Results.Ok(result);
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/url-normalize", async ([FromQuery] string url)
                => await HandleAsync(new UrlNormalizeRequest(url), CancellationToken.None))
            .WithTags("CustomEndpoints");
    }
}