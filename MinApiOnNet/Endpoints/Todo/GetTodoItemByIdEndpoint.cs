namespace MinApiOnNet.Endpoints.Todo;
public record GetTodoItemById(int Id) : EndpointRequest;

public class GetTodoItemByIdEndpoint : IEndpoint<GetTodoItemById, IResult>
{
    private readonly TodoDb _db;

    public GetTodoItemByIdEndpoint(TodoDb db) => _db = db;

    public async Task<IResult> HandleAsync(GetTodoItemById request, CancellationToken token)
    {
        var result = await _db.Todos
            .FindAsync(new object?[] {request.Id}, cancellationToken: token);

        return result is { } todo
            ? Results.Ok(todo)
            : Results.NotFound();
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/todoitems/{id}", async (
                    [FromRoute] int id,
                    CancellationToken token) =>
                await HandleAsync(new GetTodoItemById(id), token))
            .Produces<Models.Todo>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(Constants.HasNameIdentifierPolicy)
            .WithTags("TodoEndpoints");
    }
}