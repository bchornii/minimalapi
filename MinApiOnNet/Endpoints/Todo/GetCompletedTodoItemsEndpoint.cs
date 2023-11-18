using Microsoft.EntityFrameworkCore;

namespace MinApiOnNet.Endpoints.Todo;

public class GetCompletedTodoItemsEndpoint : IEndpoint<EndpointRequest, IResult>
{
    private readonly TodoDb _db;

    public GetCompletedTodoItemsEndpoint(TodoDb db) => _db = db;

    public async Task<IResult> HandleAsync(EndpointRequest request, CancellationToken token)
    {
        var result = await _db.Todos
            .Where(e => e.IsComplete)
            .Select(x => new TodoItemDto(x))
            .ToArrayAsync(token);

        return Results.Ok(result);
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .Map("/todoitems/complete", async (
                    [FromServices] TodoDb db,
                    CancellationToken token) =>
                await HandleAsync(EndpointRequest.Empty, token))
            .Produces<IList<TodoItemDto>>()
            .RequireAuthorization(Constants.HasNameIdentifierPolicy)
            .WithTags("TodoEndpoints");
    }
}