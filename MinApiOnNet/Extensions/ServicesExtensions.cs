using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinApiOnNet.Endpoints;
using MinApiOnNet.Services;

namespace MinApiOnNet.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var endpoints = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => t.GetInterfaces().Contains(typeof(IEndpoint)))
            .Where(t => !t.IsInterface);

        foreach (var endpoint in endpoints)
        {
            services.AddScoped(typeof(IEndpoint), endpoint);
        }

        return services;
    }

    public static void AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddTransient<IHighCpuUsageService, HighCpuUsageService>();
        services.AddTransient<IBlockingThreadsService, BlockingThreadsService>();
        services.AddSingleton<IMemoryLeakService, MemoryLeakService>();

        services
            .AddEndpoints()
            .AddProblemDetails()
            .AddDistributedMemoryCache()
            .AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            })
            .AddDbContext<TodoDb>(opt =>
                opt.UseInMemoryDatabase("TodoList"))
            .AddDatabaseDeveloperPageExceptionFilter()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Budget Cast Expenses API", Version = "v1"});

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer jhejehg...')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            })
            .AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN")
            .AddAuthorization(options =>
            {
                options.AddPolicy(Constants.HasNameIdentifierPolicy, policyBuilder
                    => policyBuilder.RequireClaim(ClaimTypes.NameIdentifier));
            })
            // Authentication general: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-6.0#authentication-scheme
            // Policy schemes: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/policyschemes?view=aspnetcore-6.0
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options => options.ForwardChallenge = "Google")
            .AddGoogle(options =>
            {
                options.CallbackPath = new PathString("/g-callback");
                options.ClientId = configuration["Social:Google:ClientId"];
                options.ClientSecret = configuration["Social:Google:ClientSecret"];
            });
    }
}