using MinApiOnNet.Services;

namespace MinApiOnNet.Endpoints.UnhandledExceptions;

public class ExceptionAsyncVoidEndpoint : IEndpoint<EndpointRequest, IResult>
{
    public async Task<IResult> HandleAsync(EndpointRequest request, CancellationToken token)
    {
        ExceptionsService.AsyncVoid();
        return Results.Ok();
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/exception-async-void", async ()
                => await HandleAsync(EndpointRequest.Empty, CancellationToken.None))
            .WithTags("UnhandledExceptionsEndpoints");
    }
}