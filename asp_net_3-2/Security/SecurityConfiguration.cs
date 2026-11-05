using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;
using asp_net_3_2.Models;
using asp_net_3_2.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace asp_net_3_2.Security;

public static class SecurityConfiguration
{
    public static void AddApiSecurity(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        var jwt = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
        if (environment.IsDevelopment() && string.IsNullOrEmpty(jwt.Key))
        {
            jwt.Key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
        }

        services.AddOptions<JwtOptions>().Configure(options =>
        {
            options.Issuer = jwt.Issuer;
            options.Audience = jwt.Audience;
            options.Key = jwt.Key;
            options.LifetimeMinutes = jwt.LifetimeMinutes;
        }).ValidateDataAnnotations().ValidateOnStart();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddSingleton<IAuthorizationHandler, ProfileAuthorizationHandler>();
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.Admin, policy => policy.RequireAuthenticatedUser().RequireRole("Admin"));
            options.AddPolicy(Policies.OwnerOrAdmin, policy =>
                policy.RequireAuthenticatedUser().AddRequirements(new OwnerRequirement()));
        });
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((options, settings) =>
            {
                var token = settings.Value;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = token.Issuer,
                    ValidateAudience = true,
                    ValidAudience = token.Audience,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "sub",
                    RoleClaimType = "role"
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var principal = context.Principal!;
                        if (!int.TryParse(principal.FindFirst("sub")?.Value, out var id))
                        {
                            context.Fail("Invalid subject.");
                            return;
                        }

                        var repository = context.HttpContext.RequestServices.GetRequiredService<IAccountRepository>();
                        var user = await repository.FindAccountAsync(id, context.HttpContext.RequestAborted);
                        if (user is null || !user.IsActive || user.Role.Name != principal.FindFirst("role")?.Value)
                        {
                            context.Fail("Account is inactive or its role has changed.");
                        }
                    }
                };
            });

        var origin = configuration["Cors:FrontendOrigin"]
            ?? throw new InvalidOperationException("Cors:FrontendOrigin is required.");
        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)
            || uri.Scheme is not ("http" or "https") || origin != uri.GetLeftPart(UriPartial.Authority))
        {
            throw new InvalidOperationException("Cors:FrontendOrigin must be an HTTP origin without a trailing slash.");
        }

        services.AddCors(options => options.AddPolicy("frontend", policy => policy
            .WithOrigins(origin).WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
            .WithHeaders("Content-Type", "Authorization").WithExposedHeaders("Retry-After", "X-Total-Count")));
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("login", context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }));
            options.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry))
                {
                    context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retry.TotalSeconds).ToString();
                }

                await context.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = 429,
                    Title = "Too many sign-in attempts",
                    Detail = "Wait for the next one-minute window before trying again."
                }, cancellationToken);
            };
        });
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Paste the access token from login or registration."
            });
            options.OperationFilter<AuthorizeOperationFilter>();
        });
    }
}
