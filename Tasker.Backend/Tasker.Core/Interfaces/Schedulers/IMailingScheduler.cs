using Tasker.Core.DTOs;

namespace Tasker.Core.Interfaces.Schedulers;

public interface IMailingScheduler
{
    Task StartMailingJobAsync(MailingJobDTO mailingJobDTO);
    Task StopMailingJobAsync(string objectiveId);
}
