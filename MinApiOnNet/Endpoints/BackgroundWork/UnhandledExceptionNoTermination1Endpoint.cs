namespace MinApiOnNet.Endpoints.BackgroundWork;

public class UnhandledExceptionNoTermination1Endpoint : IEndpoint<EndpointRequest, IResult>
{
    private readonly ILogger<UnhandledExceptionNoTermination1Endpoint> _logger;

    public UnhandledExceptionNoTermination1Endpoint(
        ILogger<UnhandledExceptionNoTermination1Endpoint> logger)
    {
        _logger = logger;
    }

    public async Task<IResult> HandleAsync(EndpointRequest request, CancellationToken token)
    {
        static async Task BackgroundOperationAsync(User user, ILogger<UnhandledExceptionNoTermination1Endpoint> logger)
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
            .MapGet("/unhandled-exception-no-termination-1", async (CancellationToken token)
                => await HandleAsync(EndpointRequest.Empty, token))
            .Produces(StatusCodes.Status200OK)
            .WithTags("BackgroundWorkEndpoints");
    }
}