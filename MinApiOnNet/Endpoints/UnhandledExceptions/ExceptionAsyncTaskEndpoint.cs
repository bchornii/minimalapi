using MinApiOnNet.Services;

namespace MinApiOnNet.Endpoints.UnhandledExceptions;

public class ExceptionAsyncTaskEndpoint : IEndpoint<EndpointRequest, IResult>
{
    public async Task<IResult> HandleAsync(EndpointRequest request, CancellationToken token)
    {
        await ExceptionsService.AsyncTask();
        return Results.Ok();
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/exception-async-task", async ()
                => await HandleAsync(EndpointRequest.Empty, CancellationToken.None))
            .WithTags("UnhandledExceptionsEndpoints");
    }
}