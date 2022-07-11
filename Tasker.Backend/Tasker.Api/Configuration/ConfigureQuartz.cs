using Microsoft.Data.Sqlite;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore.Common;
using System.Collections.Specialized;
using System.Data;
using Tasker.Infrastructure.Jobs;

namespace Tasker.Api.Configuration;

public static class ConfigureQuartz
{
    public static IApplicationBuilder UseQuartz(this IApplicationBuilder app)
    {
        app.ApplicationServices.GetService<IScheduler>();

        return app;
    }

    public static IServiceCollection AddQuartz(this IServiceCollection services, IConfiguration config)
    {
        DbProvider.RegisterDbMetadata("sqlite-quartz-provider", new DbMetadata()
        {
            AssemblyName = typeof(SqliteConnection).Assembly.GetName().Name,
            BindByName = true,
            ConnectionType = typeof(SqliteConnection),
            CommandType = typeof(SqliteCommand),
            ExceptionType = typeof(SqliteException),
            ParameterType = typeof(SqliteParameter),
            ParameterDbType = typeof(DbType),
            ParameterDbTypePropertyName = "DbType",
            ParameterNamePrefix = "@",
            UseParameterNamePrefixInParameterCollection = true
        });

        var properties = new NameValueCollection
        {
            ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
            ["quartz.jobStore.useProperties"] = "false",
            ["quartz.jobStore.dataSource"] = "default",
            ["quartz.jobStore.tablePrefix"] = "QRTZ_",
            ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SQLiteDelegate, Quartz",
            ["quartz.jobStore.misfireThreshold"] = "60000",
            ["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz",
            ["quartz.jobStore.txIsolationLevelSerializable"] = "true",
            ["quartz.dataSource.default.provider"] = "sqlite-quartz-provider",
            ["quartz.dataSource.default.connectionString"] = config.GetConnectionString("QuartzConnection"),
            ["quartz.serializer.type"] = "binary",
            ["quartz.threadPool.threadCount"] = "5",
            ["quartz.threadPool.threadPriority"] = "Normal",
            ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz"
        };

        var schedulerFactory = new StdSchedulerFactory(properties);
        var scheduler = schedulerFactory.GetScheduler().Result;

        scheduler.JobFactory = new JobFactory(services.BuildServiceProvider());
        scheduler.Start().Wait();

        services.AddSingleton(scheduler);

        return services;
    }
}
