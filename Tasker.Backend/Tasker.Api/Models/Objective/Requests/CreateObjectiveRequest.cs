namespace Tasker.Api.Models.Objective.Requests;

public record CreateObjectiveRequest(
    string Name,
    string Description,
    DateTime StartAt,
    int PeriodInMinutes,
    string FreeApiId,
    string? Query);
