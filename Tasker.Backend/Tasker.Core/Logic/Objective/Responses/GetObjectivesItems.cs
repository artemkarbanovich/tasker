namespace Tasker.Core.Logic.Objective.Responses;

public record GetObjectivesItems(
    string Id, 
    string Name, 
    string Description, 
    DateTime? ExecutedLastTime);
