using Tasker.Api.Middleware;
using Tasker.Core.Interfaces.Repositories;
using Tasker.Infrastructure.Data;

namespace Tasker.Api.Configuration;

public static class ConfigureApplication
{
    public static WebApplication AddApplicationConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors(policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(app.Configuration["WebClientUrl"]));
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    public static async Task AddDatabaseConfiguration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var databaseConfigurator = services.GetRequiredService<DatabaseConfigurator>();

            await databaseConfigurator.CreateDatabaseAsync();
            await databaseConfigurator.AddAdminAsync(services.GetRequiredService<IUserRepository>());
            await databaseConfigurator.SeedFreeApisAsync(services.GetRequiredService<IFreeApiRepository>());
        }
        catch (Exception ex)
        {
            services.GetRequiredService<ILogger<Program>>().LogError(ex, "Error during database configuration");
        }
    }
}
