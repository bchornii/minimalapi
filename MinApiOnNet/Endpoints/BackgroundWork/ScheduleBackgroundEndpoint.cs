namespace MinApiOnNet.Endpoints.BackgroundWork;

public class ScheduleBackgroundEndpoint : IEndpoint<EndpointRequest, IResult>
{
    private readonly ILogger<ScheduleBackgroundEndpoint> _logger;

    public ScheduleBackgroundEndpoint(ILogger<ScheduleBackgroundEndpoint> logger)
    {
        _logger = logger;
    }

    public async Task<IResult> HandleAsync(EndpointRequest request, CancellationToken token)
    {
        _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                throw new AppCustomException("Hi there!");
            }, token)
            .ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    // if(t.Exception is not null)
                    // {
                    //     t.Exception.Handle(e => true);
                    //     logger.LogWarning("Background task failed. Reason: {reason}", t.Exception.Message);
                    // }
                    _logger.LogWarning("Background task failed. Reason: {reason}", t.Exception.Message);
                }
            }, token);

        return Results.Ok();
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/schedule-background", async ([FromServices] ILogger<Program> logger, CancellationToken token)
                => await HandleAsync(EndpointRequest.Empty, token))
            .Produces(StatusCodes.Status200OK)
            .WithTags("BackgroundWorkEndpoints");
    }
}