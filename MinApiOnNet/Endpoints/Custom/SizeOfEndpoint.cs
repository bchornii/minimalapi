namespace MinApiOnNet.Endpoints.Custom;

public record SizeOfRequest(string Value) : EndpointRequest;

public class SizeOfEndpoint : IEndpoint<SizeOfRequest, IResult>
{
    public async Task<IResult> HandleAsync(SizeOfRequest request, CancellationToken token)
    {
        static int GetObjectSize(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // Calculate the size of the object by serializing it to a byte array
            using var stream = new System.IO.MemoryStream();
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
#pragma warning disable SYSLIB0011
            formatter.Serialize(stream, obj);
#pragma warning restore SYSLIB0011
            return (int)stream.Length;
        }

        var size = GetObjectSize(request.Value);

        return Results.Ok(size);
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("size-of", async ([FromQuery] string s)
                => await HandleAsync(new SizeOfRequest(s), CancellationToken.None))
            .Produces(StatusCodes.Status200OK)
            .WithTags("CustomEndpoints");
    }
}