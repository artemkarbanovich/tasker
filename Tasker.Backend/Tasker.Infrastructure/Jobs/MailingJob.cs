using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quartz;
using Tasker.Core.DTOs;
using Tasker.Core.Interfaces.Repositories;
using Tasker.Core.Interfaces.Services;

namespace Tasker.Infrastructure.Jobs;

[PersistJobDataAfterExecution]
[DisallowConcurrentExecution]
public class MailingJob : IJob
{
    private readonly IHttpService _httpService;
    private readonly ICsvService _csvService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly IObjectiveRepository _objectiveRepository;

    public MailingJob(
        IHttpService httpService,
        ICsvService csvService,
        IEmailService emailService,
        IConfiguration config,
        IObjectiveRepository objectiveRepository)
    {
        _httpService = httpService;
        _csvService = csvService;
        _emailService = emailService;
        _config = config;
        _objectiveRepository = objectiveRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var mailingJobDTO = JsonConvert.DeserializeObject<MailingJobDTO>(context.JobDetail.JobDataMap.GetString("mailingJobDTO"));

        var apiUrl =
            $"{mailingJobDTO.FreeApiUrl}" +
            $"{mailingJobDTO.FreeApiQueryKey ?? ""}" +
            $"{mailingJobDTO.ObjectiveQuery ?? ""}" +
            $"{mailingJobDTO.FreeApiRequiredQueryPrams}";

        var httpResponseBody = await _httpService.HttpGet(apiUrl, new Dictionary<string, string>
        {
            { "X-RapidAPI-Key", _config["RapidApi:Key"] },
            { "X-RapidAPI-Host", mailingJobDTO.FreeApiRapidApiHost }
        });

        var csvString = _csvService.JsonToCsvString(httpResponseBody);

        await _emailService.SendEmailWithCsvBodyAsync(
            mailingJobDTO.UserEmail,
            $"Mailing for '{mailingJobDTO.ObjectiveName}' from {DateTime.Now:dd.MM.yyyy HH:mm:ss}",
            csvString);

        var objective = await _objectiveRepository.GetObjectiveByIdAsync(mailingJobDTO.ObjectiveId);

        objective!.ExecuteObjective();

        await _objectiveRepository.UpdateObjectiveAsync(objective);
    }
}
