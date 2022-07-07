namespace Tasker.Api.Models.Objective.Requests;

public record UpdateObjectiveRequest(
    string Id,
    string Name,
    string Description,
    DateTime StartAt,
    int PeriodInMinutes,
    string FreeApiId,
    string Query);
