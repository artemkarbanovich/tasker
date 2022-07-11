namespace Tasker.Core.Logic.Statistics.Responses;

public record GetStatisticsItemResponse(
    string UserEmail,
    int ObjectivesTotalExecutedCount,
    DateTime? ObjectiveExecutedLastTime,
    int TotalObjectivesCount,
    int ObjectivesDeletedCount);
