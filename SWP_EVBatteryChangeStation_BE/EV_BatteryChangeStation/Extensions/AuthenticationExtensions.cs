using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EV_BatteryChangeStation.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtConfig");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? string.Empty))
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);
                    var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthenService>();

                    if (authService.IsTokenRevoked(token))
                    {
                        context.Fail("Token has been revoked.");
                    }

                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}
