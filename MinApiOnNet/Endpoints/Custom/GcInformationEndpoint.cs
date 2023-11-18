using System.Runtime;

namespace MinApiOnNet.Endpoints.Custom;

public class GcInformationEndpoint : IEndpoint<EndpointRequest, IResult>
{
    private readonly IConfiguration _configuration;

    public GcInformationEndpoint(IConfiguration configuration) => _configuration = configuration;

    public async Task<IResult> HandleAsync(EndpointRequest request, CancellationToken token)
    {
        var gcInfo = GC.GetGCMemoryInfo();
        return Results.Ok(new
        {
            gcInfo.Concurrent,
            gcInfo.Generation,
            HeapSizeBytes = gcInfo.HeapSizeBytes / 1024 / 1024,
            MemoryLoadBytes = gcInfo.MemoryLoadBytes / 1024 / 1024,
            TotalCommittedBytes = gcInfo.TotalCommittedBytes / 1024 / 1024,
            TotalAvailableMemoryBytes = gcInfo.TotalAvailableMemoryBytes / 1024 / 1024,
            GCSettings.IsServerGC,
            GCSettings.LargeObjectHeapCompactionMode,
        });
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/gc", async ([FromServices] IConfiguration configuration)
                => await HandleAsync(EndpointRequest.Empty, CancellationToken.None))
            .Produces(StatusCodes.Status200OK)
            .WithTags("CustomEndpoints");
    }
}