using MinApiOnNet.Services;

namespace MinApiOnNet.Endpoints.Profiling;

public class BlockingThreadsEndpoint : IEndpoint<EndpointRequest, IResult>
{
    private readonly IBlockingThreadsService _blockingThreadsService;

    public BlockingThreadsEndpoint(IBlockingThreadsService blockingThreadsService)
        => _blockingThreadsService = blockingThreadsService;

    public async Task<IResult> HandleAsync(EndpointRequest request, CancellationToken token)
    {
        _blockingThreadsService.Run();
        return Results.Ok();
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/blocking-threads", async ()
                => await HandleAsync(EndpointRequest.Empty, CancellationToken.None))
            .Produces(StatusCodes.Status200OK)
            .WithTags("ProfilingEndpoints");
    }
}