using Microsoft.EntityFrameworkCore;

namespace MinApiOnNet.Endpoints.Todo;

public class GetAllTodoItemsEndpoint : IEndpoint<EndpointRequest, IResult>
{
    private readonly TodoDb _db;

    public GetAllTodoItemsEndpoint(TodoDb db) => _db = db;

    public async Task<IResult> HandleAsync(EndpointRequest request, CancellationToken token)
    {
        var results = await _db.Todos
            .Select(x => new TodoItemDto(x))
            .ToListAsync(token);

        return Results.Ok(results);
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/todoitems", async (
                    [FromServices] TodoDb db,
                    CancellationToken token) =>
                await HandleAsync(EndpointRequest.Empty, token))
            .Produces<IList<TodoItemDto>>()
            .RequireAuthorization(Constants.HasNameIdentifierPolicy)
            .WithTags("TodoEndpoints");
    }
}