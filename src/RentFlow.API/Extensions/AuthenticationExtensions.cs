using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RentFlow.API.Authentication;
using RentFlow.Application.Abstractions.Identity;
using RentFlow.Infrastructure.Identity;

namespace RentFlow.API.Extensions;

/// <summary>
/// Registers JWT bearer authentication, role-based authorization and the
/// request-scoped <see cref="ICurrentUser"/> accessor for the API host.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Wires up JWT bearer validation from the <c>Jwt</c> configuration section,
    /// the authorization services and the current-user accessor.
    /// </summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <param name="configuration">The application configuration supplying the JWT settings.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, enabling chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the JWT configuration section is missing.</exception>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                $"The '{JwtOptions.SectionName}' configuration section is missing.");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                };
            });

        services.AddAuthorization();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}
