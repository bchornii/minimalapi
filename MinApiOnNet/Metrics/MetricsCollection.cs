namespace MinApiOnNet.Metrics;

public static class MetricsCollection
{
    /// <summary>
    /// Returns counter which counts requests to the API endpoints.
    /// </summary>
    /// <returns></returns>
    public static Counter EndpointRequestsCounter(
        string name = "minimal_api_path_counter",
        string help = "Counts requests to the Minimal API endpoints")
    {
        return Prometheus.Metrics.CreateCounter(
            name,
            help,
            new CounterConfiguration
            {
                LabelNames = new[] {"method", "endpoint"}
            });
    }
}