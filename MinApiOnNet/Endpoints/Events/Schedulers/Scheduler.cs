using System.Text;
using System.Threading.Channels;
using IntervalTree;
using MinApiOnNet.Endpoints.Events.Models;

namespace MinApiOnNet.Endpoints.Events.Schedulers;

/// <summary>
/// This is our scheduler that will consume the streams from the writers and let us
/// know which events have conflicts.
/// </summary>
public class Scheduler
{
    /// <summary>
    /// Holds the consumer side of the channel: the reader.
    /// </summary>
    private readonly ChannelReader<CalendarEvent> _reader;

    private readonly ILogger<Scheduler> _logger;

    /// <summary>
    /// The internal interval tree representation of our schedule used for collision
    /// detection by checking if we have more than 1 entry at a given interval.  The
    /// interval tree allows us to detect overlaps.
    /// </summary>
    private readonly IntervalTree<long, CalendarEvent> _schedule = new();

    /// <summary>
    /// Holds conflicts found.
    /// </summary>
    private readonly List<CalendarEvent[]> _conflicts = new();

    public IReadOnlyList<IReadOnlyCollection<CalendarEvent>> Conflicts => _conflicts;

    /// <summary>
    /// Initialize our scheduler.
    /// </summary>
    /// <param name="reader">The reader side of the channel.</param>
    /// <param name="logger"></param>
    public Scheduler(ChannelReader<CalendarEvent> reader, ILogger<Scheduler> logger)
    {
        _reader = reader;
        _logger = logger;
    }

    /// <summary>
    /// The main method for our scheduler.
    /// </summary>
    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        while (await _reader.WaitToReadAsync(cancellationToken))
        {
            if (_reader.TryRead(out var calendarEvent))
            {
                var start = calendarEvent.StartTime.ToUnixTimeSeconds();
                var end = calendarEvent.EndTime.ToUnixTimeSeconds();

                // Add the event to the interval tree
                _schedule.Add(start, end, calendarEvent);

                // Query to see if we have a conflict
                var events = _schedule.Query(start, end).ToArray();

                if (events.Length > 1)
                {
                    _conflicts.Add(events);

                    var buffer = new StringBuilder();
                    buffer.AppendLine("[CONFLICT]");

                    events.Aggregate(buffer, (buff, evt) => {
                        buff.AppendFormat($"  {evt.StartTime:yyyy-MM-dd HH:mm} - {evt.EndTime:yyyy-MM-dd HH:mm}: {evt.Title}");
                        buff.Append(Environment.NewLine);
                        return buff;
                    });

                    _logger.LogInformation($"{buffer}--------");
                }
            }
        }
    }
}