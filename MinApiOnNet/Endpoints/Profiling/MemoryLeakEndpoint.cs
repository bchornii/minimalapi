using MinApiOnNet.Services;

namespace MinApiOnNet.Endpoints.Profiling;

public class MemoryLeakEndpoint : IEndpoint<EndpointRequest, IResult>
{
    private readonly IMemoryLeakService _memoryLeakService;

    public MemoryLeakEndpoint(IMemoryLeakService memoryLeakService) => _memoryLeakService = memoryLeakService;

    public async Task<IResult> HandleAsync(EndpointRequest request, CancellationToken token)
    {
        _memoryLeakService.Run();
        return Results.Ok();
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/memory-leak", async (CancellationToken token) =>
                await HandleAsync(EndpointRequest.Empty, token))
            .Produces(StatusCodes.Status200OK)
            .WithTags("ProfilingEndpoints");
    }
}