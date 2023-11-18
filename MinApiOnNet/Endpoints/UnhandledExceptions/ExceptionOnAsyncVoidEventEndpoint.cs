using MinApiOnNet.AsyncExtensions;

namespace MinApiOnNet.Endpoints.UnhandledExceptions;

public record ExceptionOnAsyncVoidEventRequest(bool isAsync) : EndpointRequest;

public class ExceptionOnAsyncVoidEventEndpoint : IEndpoint<ExceptionOnAsyncVoidEventRequest, IResult>
{
    public async Task<IResult> HandleAsync(ExceptionOnAsyncVoidEventRequest request, CancellationToken token)
    {
        var instance = new AppState();

        instance.DemoEvent += async (sender, eventArgs) =>
        {
            await Task.Delay(50);
            throw new InvalidOperationException("Sabotage!");
        };

        try
        {
            if (!request.isAsync)
            {
                instance.Invoke();
            }
            else
            {
                instance.InvokeAsync();
            }

            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.Ok($"Exception: - {e.Message}");
        }
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/exception-async-void-event", async ([FromQuery] bool invoke) =>
                await HandleAsync(new ExceptionOnAsyncVoidEventRequest(invoke), CancellationToken.None))
            .WithTags("UnhandledExceptionsEndpoints");
    }
}