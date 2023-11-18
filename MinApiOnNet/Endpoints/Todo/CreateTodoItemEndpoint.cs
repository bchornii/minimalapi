namespace MinApiOnNet.Endpoints.Todo;

public record CreateTodoItemRequest(TodoItemDto ItemDto) : EndpointRequest;

public class CreateTodoItemEndpoint : IEndpoint<CreateTodoItemRequest, IResult>
{
    private readonly TodoDb _db;

    public CreateTodoItemEndpoint(TodoDb db) => _db = db;

    public async Task<IResult> HandleAsync(CreateTodoItemRequest request, CancellationToken token)
    {
        var todo = request.ItemDto;
        todo = null;

        var todoItem = new Models.Todo
        {
            IsComplete = todo.IsComplete,
            Name = todo.Name
        };
        _db.Todos.Add(todoItem);
        await _db.SaveChangesAsync(token);

        return Results.Created($"/todoitems/{todo.Id}", todo);
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapPost("/todoitems", async (
                    [FromBody] TodoItemDto todo,
                    CancellationToken token) =>
                await HandleAsync(new CreateTodoItemRequest(todo), token))
            .Produces(StatusCodes.Status201Created)
            .RequireAuthorization(Constants.HasNameIdentifierPolicy)
            .WithTags("TodoEndpoints");
    }
}