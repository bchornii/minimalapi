using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace MinApiOnNet.Endpoints.Expressions;

public record FilterRequest(string? PropertyName, string? Value) : EndpointRequest;

public class FilterEndpoint : IEndpoint<FilterRequest, IResult>
{
    public async Task<IResult> HandleAsync(FilterRequest request, CancellationToken token)
    {
        static UnaryExpression GetValueExpression(string propertyName, string val, ParameterExpression param)
        {
            var member = Expression.Property(param, propertyName);
            var propertyType = ((PropertyInfo)member.Member).PropertyType;
            var converter = TypeDescriptor.GetConverter(propertyType);
            if (!converter.CanConvertFrom(typeof(string)))
            {
                throw new NotSupportedException();
            }

            var propertyValue = converter.ConvertFromInvariantString(val);
            var constant = Expression.Constant(propertyValue);
            return Expression.Convert(constant, propertyType);
        }

        static Func<User, bool> GetDynamicQueryWithExpressionTrees(string propertyName, string val)
        {
            var param = Expression.Parameter(typeof(User), "x");

            #region Convert to specific data type
            MemberExpression member = Expression.Property(param, propertyName);
            UnaryExpression valueExpression = GetValueExpression(propertyName, val, param);
            #endregion

            Expression body = Expression.Equal(member, valueExpression);
            var final = Expression.Lambda<Func<User, bool>>(body: body, parameters: param);
            return final.Compile();
        }

        var userData = User.UserDataSeed();
        var propName = request.PropertyName;
        var value = request.Value;

        if (string.IsNullOrWhiteSpace(propName))
        {
            return Results.Ok(userData);
        }

        var dn = GetDynamicQueryWithExpressionTrees(propName, value!);
        var output = userData.Where(dn).ToList();

        return Results.Ok(output);
    }

    public void Map(IEndpointRouteBuilder app)
    {
        app
            .MapGet("/filter", (
                [FromServices] ILogger<Program> logger,
                [FromQuery] string? propName,
                [FromQuery] string? value) =>
            {

            })
            .Produces<IList<User>>()
            .Produces(StatusCodes.Status200OK)
            .WithTags("ExpressionTreesEndpoints");
    }
}