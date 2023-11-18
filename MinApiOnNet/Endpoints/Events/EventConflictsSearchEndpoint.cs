using System.Diagnostics;
using System.Threading.Channels;
using MinApiOnNet.Endpoints.Events.Models;
using MinApiOnNet.Endpoints.Events.Providers;
using MinApiOnNet.Endpoints.Events.Schedulers;

namespace MinApiOnNet.Endpoints.Events;

public class EventConflictsSearchEndpoint : IEndpoint<EndpointRequest, IResult>
{
    private readonly ILogger<Scheduler> _schedulerLogger;
    private readonly ILogger<IEndpoint> _endpointLogger;

    public EventConflictsSearchEndpoint(
        ILogger<Scheduler> schedulerLogger,
        ILogger<IEndpoint> endpointLogger)
    {
        _schedulerLogger = schedulerLogger;
        _endpointLogger = endpointLogger;
    }

    public async Task<IResult> HandleAsync(EndpointRequest request, CancellationToken token)
    {
        // Create our channel.
        var channel = Channel.CreateUnbounded<CalendarEvent>();
        var reader = channel.Reader;
        var writer = channel.Writer;

        var scheduler = new Scheduler(reader, logger: _schedulerLogger);
        var googleProvider = new GoogleEventProvider(writer);
        var outlookProvider = new OutlookEventProvider(writer);
        var appleProvider = new AppleEventProvider(writer);

        // Start our scheduler.
        var schedulerProcessingTask = scheduler.ProcessAsync(token);

        // Create our calendar retrieval tasks.
        var googleCalendarTask = googleProvider.RunAsync(token);
        var outlookCalendarTask = outlookProvider.RunAsync(token);
        var appleCalendarTask = appleProvider.RunAsync(token);

        // Our stopwatch to check the start/end time.
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        // Now start our tasks execute and complete concurrently.
        await Task
            .WhenAll(googleCalendarTask, outlookCalendarTask, appleCalendarTask)
            .ContinueWith(_ => writer.Complete());

        stopwatch.Stop();

        await schedulerProcessingTask;

        // Write out our time metrics; should be around ~3s even though we
        // make total of 9 simulated calls with up to 1s each.
        var elapsedSeconds = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds).TotalSeconds;
        _endpointLogger.LogInformation($"Completed in {elapsedSeconds}s");

        var conflicts = scheduler.Conflicts;
        return Results.Ok(conflicts);
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("events/conflicts",
            async (CancellationToken token) =>
                await HandleAsync(EndpointRequest.Empty, token))
            .WithTags("EventsSchedulingEndpoints");
    }
}