namespace MinApiOnNet.Endpoints;

public record EndpointRequest
{
    public static readonly EndpointRequest Empty = new();
}

public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}

public interface IEndpoint<in TRequest, TResult> : IEndpoint
    where TRequest : EndpointRequest
{
    Task<TResult> HandleAsync(TRequest request, CancellationToken token);
}