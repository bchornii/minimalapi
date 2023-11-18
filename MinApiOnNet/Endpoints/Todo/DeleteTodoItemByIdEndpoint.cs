namespace MinApiOnNet.Endpoints.Todo;

public record DeleteTodoItemByIdRequest(int Id) : EndpointRequest;

public class DeleteTodoItemByIdEndpoint : IEndpoint<DeleteTodoItemByIdRequest, IResult>
{
    private readonly TodoDb _db;

    public DeleteTodoItemByIdEndpoint(TodoDb db) => _db = db;

    public async Task<IResult> HandleAsync(DeleteTodoItemByIdRequest request, CancellationToken token)
    {
        if (await _db.Todos.FindAsync(new object?[] {request.Id}, cancellationToken: token) is { } todo)
        {
            _db.Todos.Remove(todo);
            await _db.SaveChangesAsync(token);
            return Results.Ok(new TodoItemDto(todo));
        }

        return Results.NotFound();
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapDelete("/todoitems/{id}",
                async (int id, CancellationToken token) =>
                    await HandleAsync(new DeleteTodoItemByIdRequest(id), token))
            .Produces<TodoItemDto>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(Constants.HasNameIdentifierPolicy)
            .WithTags("TodoEndpoints");
    }
}