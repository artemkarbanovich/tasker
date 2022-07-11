using Quartz;
using Tasker.Core.DTOs;
using Tasker.Core.Interfaces.Schedulers;
using Tasker.Infrastructure.Jobs;

namespace Tasker.Infrastructure.Schedulers;

public class MailingScheduler : IMailingScheduler
{
    private readonly IScheduler _scheduler;

    public MailingScheduler(IScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public async Task StartMailingJobAsync(MailingJobDTO mailingJobDTO)
    {
        IJobDetail job = JobBuilder.Create<MailingJob>()
            .UsingJobData("mailingJobDTO", Newtonsoft.Json.JsonConvert.SerializeObject(mailingJobDTO))
            .WithIdentity(mailingJobDTO.ObjectiveId, "mailing-job")
            .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity(mailingJobDTO.ObjectiveId, "mailing-trigger")
            .StartAt(mailingJobDTO.ObjectiveStartAt)
            .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromMinutes(mailingJobDTO.ObjectivePeriodInMinutes)).RepeatForever())
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
    }

    public async Task StopMailingJobAsync(string objectiveId)
    {
        await _scheduler.UnscheduleJob(new TriggerKey(objectiveId, "mailing-trigger"));
        await _scheduler.DeleteJob(new JobKey(objectiveId, "mailing-job"));
    }
}
