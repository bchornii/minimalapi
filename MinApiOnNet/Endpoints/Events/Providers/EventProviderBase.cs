using System.Globalization;
using System.Threading.Channels;
using MinApiOnNet.Endpoints.Events.Models;

namespace MinApiOnNet.Endpoints.Events.Providers;

/// <summary>
/// Represents a provider which connects to a remote origin to retrieve a list of
/// calendar events.  In this case, we are just using this as a mock proxy for an
/// actual API call.
/// </summary>
public abstract class EventProviderBase
{
  private static readonly string DatePattern = "yyyy-MM-dd HH:mm";
  private static readonly Random Random = new();

  private readonly ChannelWriter<CalendarEvent> _writer;
  private int _index = 0;

  /// <summary>
  /// Base constructor
  /// </summary>
  protected EventProviderBase(ChannelWriter<CalendarEvent> writer)
    => _writer = writer;

  /// <summary>
  /// Given a date and time string, create a DateTimeOffset instance.
  /// </summary>
  /// <param name="dateTime">The string value yyyy-MM-dd HH:mm</param>
  /// <returns>A DateTimeOffset representing the specified time.</returns>
  protected DateTimeOffset MakeDate(string dateTime)
    => DateTimeOffset.ParseExact(dateTime, DatePattern, CultureInfo.InstalledUICulture.DateTimeFormat);

  /// <summary>
  /// The actual logic to do the retrieval would go here; in this code, we
  /// are just simulating.
  /// </summary>
  public async Task RunAsync(CancellationToken cancellationToken)
  {
    do {
      var events = await GetCalendarEventsAsync(cancellationToken);

      if (events.Length == 0)
      {
        break;
      }

      foreach(var e in events)
      {
        await _writer.WriteAsync(e, cancellationToken);
      }
    } while (true);
  }

  /// <summary>
  /// Inheriting classes to provide a list of events for iteration.
  /// </summary>
  protected abstract IEnumerable<CalendarEvent>[] Events { get; }

  /// <summary>
  /// Provides a list of events; this is only for simulation.
  /// </summary>
  private async Task<CalendarEvent[]> GetCalendarEventsAsync(CancellationToken cancellationToken)
  {
    // Simulate API call delay.
    await Task.Delay(Random.Next(250, 1000), cancellationToken);

    if (_index > Events.Length - 1)
    {
      return Array.Empty<CalendarEvent>(); // Simulate no more events.
    }

    var events = Events[_index];
    _index++;

    return events.ToArray();
  }
}