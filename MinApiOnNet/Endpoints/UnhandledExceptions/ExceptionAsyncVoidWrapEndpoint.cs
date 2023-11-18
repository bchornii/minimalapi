using MinApiOnNet.Services;

namespace MinApiOnNet.Endpoints.UnhandledExceptions;

public class ExceptionAsyncVoidWrapEndpoint : IEndpoint<EndpointRequest, IResult>
{
    public async Task<IResult> HandleAsync(EndpointRequest request, CancellationToken token)
    {
        await Task.Run(ExceptionsService.AsyncVoid);
        return Results.Ok();
    }

    public void Map(IEndpointRouteBuilder app)
    {

        app
            .MapGet("/exception-async-void-wrap", async ()
                => await HandleAsync(EndpointRequest.Empty, CancellationToken.None))
            .WithTags("UnhandledExceptionsEndpoints");
    }
}