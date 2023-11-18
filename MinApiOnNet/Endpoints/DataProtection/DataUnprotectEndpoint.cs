using Microsoft.AspNetCore.DataProtection;

namespace MinApiOnNet.Endpoints.DataProtection;

public record DataUnprotectRequest(string ProtectedTerm) : EndpointRequest;

public class DataUnprotectEndpoint : IEndpoint<DataUnprotectRequest, IResult>
{
    private readonly IDataProtectionProvider _provider;

    public DataUnprotectEndpoint(IDataProtectionProvider provider) => _provider = provider;

    public async Task<IResult> HandleAsync(DataUnprotectRequest request, CancellationToken token)
    {
        var protector = _provider.CreateProtector($"{nameof(Program)}.protector");
        var protectedTerm = protector.Protect(request.ProtectedTerm);

        return Results.Ok(new
        {
            OriginalTerm = request.ProtectedTerm,
            ProtectedTerm = protectedTerm,
        });
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/unprotect", async ([FromQuery] string protectedTerm)
                => await HandleAsync(new DataUnprotectRequest(protectedTerm), CancellationToken.None))
            .Produces(StatusCodes.Status200OK)
            .WithTags("DataProtectionEndpoints");
    }
}