using System.Globalization;

namespace MinApiOnNet.Endpoints.Custom;

public record SessionDataRequest(HttpContext HttpContext) : EndpointRequest;

public class SessionDataEndpoint : IEndpoint<SessionDataRequest, IResult>
{
    public async Task<IResult> HandleAsync(SessionDataRequest request, CancellationToken token)
    {
        var dt = DateTime.Now;
        var cachedDt = request.HttpContext.Session.GetString("time-stamp");
        if (string.IsNullOrWhiteSpace(cachedDt))
        {
            request.HttpContext.Session.SetString("time-stamp", dt.ToString(CultureInfo.InvariantCulture));
        }

        return Results.Ok(new
        {
            Timestamp = request.HttpContext.Session.GetString("time-stamp")
        });
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/session-data", async (HttpContext context, CancellationToken token)
                => await HandleAsync(new SessionDataRequest(context), token))
            .WithTags("CustomEndpoints");
    }
}