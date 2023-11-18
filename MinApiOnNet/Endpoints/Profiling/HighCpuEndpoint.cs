using MinApiOnNet.Services;

namespace MinApiOnNet.Endpoints.Profiling;

public class HighCpuEndpoint : IEndpoint<EndpointRequest, IResult>
{
    private readonly IHighCpuUsageService _highCpuUsageService;

    public HighCpuEndpoint(IHighCpuUsageService highCpuUsageService)
        => _highCpuUsageService = highCpuUsageService;

    public async Task<IResult> HandleAsync(EndpointRequest request, CancellationToken token)
    {
        _highCpuUsageService.Run();
        return Results.Ok();
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/high-cpu", async () =>
                await HandleAsync(EndpointRequest.Empty, CancellationToken.None))
            .Produces(StatusCodes.Status200OK)
            .WithTags("ProfilingEndpoints");
    }
}