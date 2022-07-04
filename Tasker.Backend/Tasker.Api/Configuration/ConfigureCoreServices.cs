using Tasker.Core.Logic.Account;
using Tasker.Core.Logic.Token;

namespace Tasker.Api.Configuration;

public static class ConfigureCoreServices
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<AccountService>();
        services.AddScoped<TokenService>();

        return services;
    }
}
