using System.Globalization;
using Tasker.Api.Configuration;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSerilog(builder.Configuration);
builder.Services
    .AddCoreServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices();

var app = builder.Build();

app.AddApplicationConfiguration();
await app.AddDatabaseConfiguration();
await app.RunAsync();
