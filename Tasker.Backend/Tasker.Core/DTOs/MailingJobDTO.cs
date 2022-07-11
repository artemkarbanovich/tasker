namespace Tasker.Core.DTOs;

public record MailingJobDTO(
    string UserId,
    string UserEmail,
    string FreeApiUrl,
    string FreeApiRapidApiHost,
    string? FreeApiQueryKey,
    string? FreeApiRequiredQueryPrams,
    string ObjectiveId,
    string ObjectiveName,
    DateTime ObjectiveStartAt,
    int ObjectivePeriodInMinutes,
    string? ObjectiveQuery);
