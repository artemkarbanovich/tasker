namespace Tasker.Core.Logic.FreeApi.Responses;

public record GetFreeApisItemResponse(
    string Id, 
    string Name,
    string ApiDescription,
    string? ApiIconUrl,
    bool IsQueryRequired,
    string? QueryDescription);
