namespace Tasker.Core.Logic.Objective.Responses;

public record GetObjectiveResponse(
    string Id, 
    string Name, 
    string Description,
    DateTime CreationTime,
    DateTime LatestUpdateTime,
    DateTime StartAt,
    int PeriodInMinutes,
    string FreeApiId,
    string? Query,
    int ExecutedCount,
    DateTime? ExecutedLastTime);
