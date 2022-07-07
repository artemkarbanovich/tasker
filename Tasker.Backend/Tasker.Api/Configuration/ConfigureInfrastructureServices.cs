using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tasker.Core.Interfaces.Repositories;
using Tasker.Core.Interfaces.Services;
using Tasker.Infrastructure.Data;
using Tasker.Infrastructure.Data.Repositories;
using Tasker.Infrastructure.Mapping;
using Tasker.Infrastructure.Services;

namespace Tasker.Api.Configuration;

public static class ConfigureInfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["Jwt:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SecretKey"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
            opt.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
        });

        var sqliteConnection = new SqliteConnection(config.GetConnectionString("DefaultConnection"));

        services.AddScoped(opt => new DatabaseConfigurator(sqliteConnection));
        services.AddScoped<IUserRepository, UserRepository>(opt => new UserRepository(sqliteConnection));
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>(opt => new RefreshTokenRepository(sqliteConnection));
        services.AddScoped<IFreeApiRepository, FreeApiRepository>(opt => new FreeApiRepository(sqliteConnection));
        services.AddScoped<IObjectiveRepository, ObjectiveRepository>(opt => new ObjectiveRepository(sqliteConnection));

        services.AddScoped<ITokenService, TokenService>();

        services.AddAutoMapper(typeof(MapperProfile).Assembly);

        return services;
    }
}
