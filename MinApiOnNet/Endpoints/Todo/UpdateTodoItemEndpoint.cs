namespace MinApiOnNet.Endpoints.Todo;

public record UpdateTodoItemRequest(int Id, TodoItemDto ItemDto) : EndpointRequest;

public class UpdateTodoItemEndpoint : IEndpoint<UpdateTodoItemRequest, IResult>
{
    private readonly TodoDb _db;

    public UpdateTodoItemEndpoint(TodoDb db)
    {
        _db = db;
    }

    public async Task<IResult> HandleAsync(UpdateTodoItemRequest request, CancellationToken token)
    {
        var dto = request.ItemDto;

        var todo = await _db.Todos
            .FindAsync(new object?[] {request.Id}, cancellationToken: token);

        if (todo is null)
        {
            return Results.NotFound();
        }

        todo.Name = dto.Name;
        todo.IsComplete = dto.IsComplete;

        await _db.SaveChangesAsync(token);

        return Results.NoContent();
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapPut("/todoitems/{id}", async (
                    [FromRoute] int id,
                    [FromBody] TodoItemDto dto,
                    CancellationToken token) =>
                await HandleAsync(new UpdateTodoItemRequest(id, dto), token))
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(Constants.HasNameIdentifierPolicy)
            .WithTags("TodoEndpoints");
    }
}