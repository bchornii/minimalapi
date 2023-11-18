using System.Globalization;

namespace MinApiOnNet.Endpoints.Telemetry;

public record RollDiceEndpointRequest(string? Player) : EndpointRequest;

public class RollDiceEndpoint : IEndpoint<RollDiceEndpointRequest, IResult>
{
    private readonly ILogger<RollDiceEndpoint> _logger;

    public RollDiceEndpoint(ILogger<RollDiceEndpoint> logger)
        => _logger = logger;

    public async Task<IResult> HandleAsync(RollDiceEndpointRequest request, CancellationToken token)
    {
        HandleRollDice(request.Player);
        return Results.Ok();
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/rolldice/{player?}", async ([FromRoute] string? player)
                => await HandleAsync(new RollDiceEndpointRequest(player), CancellationToken.None))
            .Produces(StatusCodes.Status200OK)
            .WithTags("CustomEndpoints");
    }

    private int RollDice()
    {
        return Random.Shared.Next(1, 7);
    }

    private string HandleRollDice(string? player)
    {
        var result = RollDice();

        if (string.IsNullOrEmpty(player))
        {
            _logger.LogInformation("Anonymous player is rolling the dice: {result}", result);
        }
        else
        {
            _logger.LogInformation("{player} is rolling the dice: {result}", player, result);
        }

        return result.ToString(CultureInfo.InvariantCulture);
    }
}