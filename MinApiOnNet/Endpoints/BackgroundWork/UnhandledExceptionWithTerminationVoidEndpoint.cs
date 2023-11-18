namespace MinApiOnNet.Endpoints.BackgroundWork;

public class UnhandledExceptionWithTerminationVoidEndpoint : IEndpoint<EndpointRequest, IResult>
{
    private readonly ILogger<UnhandledExceptionWithTerminationVoidEndpoint> _logger;

    public UnhandledExceptionWithTerminationVoidEndpoint(
        ILogger<UnhandledExceptionWithTerminationVoidEndpoint> logger)
    {
        _logger = logger;
    }

    public async Task<IResult> HandleAsync(EndpointRequest request, CancellationToken token)
    {
        // async void is almost always is a recipe to terminate the running process
        // when an unhandled exception occurs.
        static async void BackgroundOperationAsync(User user, ILogger<UnhandledExceptionWithTerminationVoidEndpoint> logger)
        {
            await Task.Delay(1000);
            logger.LogInformation(user.FirstName);
        }

        BackgroundOperationAsync(null!, _logger);
        return Results.Ok();
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/unhandled-exception-with-termination-void", async (CancellationToken token)
                => await HandleAsync(EndpointRequest.Empty, token))
            .Produces(StatusCodes.Status200OK)
            .WithTags("BackgroundWorkEndpoints");
    }
}