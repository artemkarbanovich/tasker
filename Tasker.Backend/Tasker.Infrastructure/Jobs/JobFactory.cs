using Quartz;
using Quartz.Spi;

namespace Tasker.Infrastructure.Jobs;

public class JobFactory : IJobFactory
{
    private readonly IServiceProvider _serviceProvider;

    public JobFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler) => _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;

    public void ReturnJob(IJob job) => (job as IDisposable)?.Dispose();
}
