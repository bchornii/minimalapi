var builder = WebApplication.CreateBuilder(args);

builder
    .Host
    .UseSharedSerilogConfiguration();

var jwtSettings = builder.Configuration
    .GetSection($"SecuritySettings:{nameof(JwtSettings)}")
    .Get<JwtSettings>();

#region Services & Pipeline

builder
    .Services
    .AddApplicationServices(builder.Configuration);

var app = builder.Build();

var endpointRequestsCounter = MetricsCollection
    .EndpointRequestsCounter();

app
    .UseCustomExceptionHandler(
        isDevelopment: app.Environment.IsDevelopment())
    .UseSwagger()
    .UseSwaggerUI()
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization()
    .UseSession()
    .Use(async (context, next) =>
    {
        endpointRequestsCounter
            .WithLabels(context.Request.Method, context.Request.Path)
            .Inc();

        await next();
    })
    .UseMetricServer();

#endregion

app.Map("/", [AllowAnonymous] () =>
    Results.Redirect("/swagger"));

app.MapEndpoints();
app.Run();