namespace MinApiOnNet.Endpoints.Custom;

public record UploadStreamRequest(HttpRequest HttpRequest) : EndpointRequest;

public class UploadStreamEndpoint : IEndpoint<UploadStreamRequest, IResult>
{
    private readonly IConfiguration _configuration;

    public UploadStreamEndpoint(IConfiguration configuration)
        => _configuration = configuration;

    public async Task<IResult> HandleAsync(UploadStreamRequest request, CancellationToken token)
    {
        var filePath = Path.Combine(_configuration["StoredFilesPath"] ?? @"D:\", Path.GetRandomFileName());

        await using var writeStream = File.Create(filePath);
        await request.HttpRequest.BodyReader.CopyToAsync(writeStream, token);

        return Results.Ok();
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapPost("/uploadstream", async (HttpRequest request, CancellationToken token)
                => await HandleAsync(new UploadStreamRequest(request), token))
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(Constants.HasNameIdentifierPolicy)
            .WithTags("CustomEndpoints");
    }
}