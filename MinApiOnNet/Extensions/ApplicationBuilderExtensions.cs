using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;

namespace MinApiOnNet.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder applicationBuilder, bool isDevelopment)
    {
        // Handler for uncaught exceptions. Otherwise AppInsights will report a 200 for failed requests instead of 500
        // Source: https://stackoverflow.com/a/40543603/1537195
        applicationBuilder.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = MediaTypeNames.Text.Plain;

                if (context.RequestServices.GetService<IProblemDetailsService>() is
                    { } problemDetailsService)
                {
                    if (context.Features.Get<IExceptionHandlerFeature>() is { } exceptionHandlerFeature)
                    {
                        await problemDetailsService.WriteAsync(new ProblemDetailsContext
                        {
                            HttpContext = context,
                            ProblemDetails = new ProblemDetails
                            {
                                Instance = "ApiExceptionHandling",
                                Title = "An error occurred while processing your request.",
                                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                                Detail = "Unhandled exception",
                                Extensions =
                                {
                                    { "errorMessage", exceptionHandlerFeature.Error.Message },
                                    {
                                        "stackTrace",
                                        isDevelopment ? exceptionHandlerFeature.Error.StackTrace : string.Empty
                                    }
                                }
                            },
                        });
                    }
                }
            });
        });
        
        return applicationBuilder;
    }
}