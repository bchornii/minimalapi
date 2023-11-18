using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Sinks.SystemConsole.Themes;

namespace MinApiOnNet.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseSharedSerilogConfiguration(this IHostBuilder builder)
    {
        return builder.UseSerilog((context, services, configuration) =>
        {
            var serviceName = context.Configuration.GetValue<string>("ServiceName");

            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithSpan()
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                    .WithDefaultDestructurers()
                    .WithDestructurers(new IExceptionDestructurer[]
                    {
                        new DbUpdateExceptionDestructurer()
                    }))
                .Enrich.WithMachineName()
                .Enrich.WithCorrelationIdHeader("X-Correlation-ID")
                .Enrich.WithProperty("ServiceName", serviceName);

            configuration
                .WriteTo.Async(x => x.Console(
                    restrictedToMinimumLevel: LogEventLevel.Information, 
                    theme: AnsiConsoleTheme.Code));
        });
    }
}