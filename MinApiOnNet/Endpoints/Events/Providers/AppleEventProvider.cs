using System.Threading.Channels;
using MinApiOnNet.Endpoints.Events.Models;

namespace MinApiOnNet.Endpoints.Events.Providers;

/// <summary>
/// A mock provider of events from Apple iCloud.
/// </summary>
public class AppleEventProvider : EventProviderBase
{
  /// <summary>
  /// We use this to hold our list of events that we are going to
  /// return.
  /// </summary>
  protected override IEnumerable<CalendarEvent>[] Events { get; }

  /// <summary>
  /// Simulates iCloud event source.
  /// </summary>
  public AppleEventProvider(ChannelWriter<CalendarEvent> writer) : base(writer)
  {
    Events = new IEnumerable<CalendarEvent>[]
    {
      // First set.
      new CalendarEvent[]
      {
        new (
          MakeDate("2022-11-15 08:15"),
          MakeDate("2022-11-15 08:45"),
          "Drop off Amy"
        ),
        new (
          MakeDate("2022-11-15 11:00"),
          MakeDate("2022-11-15 11:30"),
          "Followup with accountant on tax prep"
        ),
      },

      // Second set.
      new CalendarEvent[]
      {
        new (
          MakeDate("2022-11-15 13:00"),
          MakeDate("2022-11-15 14:00"),
          "Late lunch"
        ),
        new (
          MakeDate("2022-11-15 14:50"),
          MakeDate("2022-11-15 14:55"),
          "Dont forget to take meds!"
        ),
        new (
          MakeDate("2022-11-15 16:15"),
          MakeDate("2022-11-15 16:30"),
          "Pick up Amy"
        ),
      },

      // Third set.
      new CalendarEvent[]
      {
        new (
          MakeDate("2022-11-15 20:15"),
          MakeDate("2022-11-15 20:30"),
          "Prep the guest bedroom for in laws"
        ),
        new (
          MakeDate("2022-11-15 22:00"),
          MakeDate("2022-11-15 22:45"),
          "Medidate before bed"
        ),
      },
    };
  }
}