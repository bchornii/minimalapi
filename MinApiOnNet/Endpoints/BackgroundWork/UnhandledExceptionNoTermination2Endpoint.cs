namespace MinApiOnNet.Endpoints.BackgroundWork;

public class UnhandledExceptionNoTermination2Endpoint : IEndpoint<EndpointRequest, IResult>
{
    private readonly ILogger<UnhandledExceptionNoTermination2Endpoint> _logger;

    public UnhandledExceptionNoTermination2Endpoint(
        ILogger<UnhandledExceptionNoTermination2Endpoint> logger)
    {
        _logger = logger;
    }

    public async Task<IResult> HandleAsync(EndpointRequest request, CancellationToken token)
    {
        static async Task BackgroundOperationAsync(User user, ILogger<UnhandledExceptionNoTermination2Endpoint> logger)
        {
            await Task.Delay(1000);
            logger.LogInformation(user.FirstName);
        }

        Task.Run(() => BackgroundOperationAsync(null!, _logger));
        return Results.Ok();
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/unhandled-exception-no-termination-2", async (CancellationToken token)
                => await HandleAsync(EndpointRequest.Empty, token))
            .Produces(StatusCodes.Status200OK)
            .WithTags("BackgroundWorkEndpoints");;
    }
}