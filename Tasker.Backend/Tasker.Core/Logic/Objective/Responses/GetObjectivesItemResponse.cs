namespace Tasker.Core.Logic.Objective.Responses;

public record GetObjectivesItemResponse(
    string Id, 
    string Name, 
    string Description, 
    DateTime? ExecutedLastTime);
