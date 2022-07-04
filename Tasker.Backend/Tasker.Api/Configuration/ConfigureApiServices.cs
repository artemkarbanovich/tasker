using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Tasker.Api.Filters;

namespace Tasker.Api.Configuration;

public static class ConfigureApiServices
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddControllers(opt =>
        {
            opt.Filters.Add<ValidationFilter>();
        });
        services.AddCors();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddFluentValidation(opt =>
        {
            opt.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
        });

        services.Configure<RouteOptions>(opt => opt.LowercaseUrls = true);
        services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);

        return services;
    }
}
