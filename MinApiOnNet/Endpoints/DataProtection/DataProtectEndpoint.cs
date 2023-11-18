using Microsoft.AspNetCore.DataProtection;

namespace MinApiOnNet.Endpoints.DataProtection;

public record DataProtectRequest(string Term) : EndpointRequest;

public class DataProtectEndpoint : IEndpoint<DataProtectRequest, IResult>
{
    private readonly IDataProtectionProvider _provider;

    public DataProtectEndpoint(IDataProtectionProvider provider) => _provider = provider;

    public async Task<IResult> HandleAsync(DataProtectRequest request, CancellationToken token)
    {
        var protector = _provider.CreateProtector($"{nameof(Program)}.protector");
        var protectedTerm = protector.Protect(request.Term);

        return Results.Ok(new
        {
            OriginalTerm = request.Term,
            ProtectedTerm = protectedTerm,
        });
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/protect", async ([FromQuery] string term) =>
                await HandleAsync(new DataProtectRequest(term), CancellationToken.None))
            .Produces(StatusCodes.Status200OK)
            .WithTags("DataProtectionEndpoints");
    }
}