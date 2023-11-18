using MinApiOnNet.Endpoints;

namespace MinApiOnNet.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void MapEndpoints(this WebApplication builder)
    {
        using var scope = builder.Services.CreateScope();

        var endpoints = scope.ServiceProvider.GetServices<IEndpoint>();

        foreach (var endpoint in endpoints)
        {
            endpoint.Map(builder);
        }
    }
}