namespace Tasker.Core.Entities;

public class Objective
{
    public Objective(
        string userId,
        string name,
        string description,
        DateTime startAt,
        int periodInMinutes,
        string freeApiId,
        string? query)
    {
        UserId = userId;
        Name = name;
        Description = description;
        StartAt = startAt;
        PeriodInMinutes = periodInMinutes;
        FreeApiId = freeApiId;
        Query = query;
    }

    public Objective(
        string id,
        string userId,
        string name,
        string description,
        DateTime creationTime,
        DateTime latestUpdateTime,
        DateTime startAt,
        int periodInMinutes,
        string freeApiId,
        string? query,
        int executedCount,
        DateTime? executedLastTime,
        bool isDeleted)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Description = description;
        CreationTime = creationTime;
        LatestUpdateTime = latestUpdateTime;
        StartAt = startAt;
        PeriodInMinutes = periodInMinutes;
        FreeApiId = freeApiId;
        Query = query;
        ExecutedCount = executedCount;
        ExecutedLastTime = executedLastTime;
        IsDeleted = isDeleted;
    }

    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string UserId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime CreationTime { get; private set; } = DateTime.UtcNow;
    public DateTime LatestUpdateTime { get; private set; } = DateTime.UtcNow;
    public DateTime StartAt { get; private set; }
    public int PeriodInMinutes { get; private set; }
    public string FreeApiId { get; private set; }
    public string? Query { get; private set; }
    public int ExecutedCount { get; private set; } = 0;
    public DateTime? ExecutedLastTime { get; private set; } = null;
    public bool IsDeleted { get; private set; } = false;

    public void UpdateObjective(
        string name, 
        string description, 
        DateTime startAt, 
        int periodInMinutes,
        string freeApiId, 
        string? query)
    {
        Name = name;
        Description = description;
        LatestUpdateTime = DateTime.UtcNow;
        StartAt = startAt;
        PeriodInMinutes = periodInMinutes;
        FreeApiId = freeApiId;
        Query = query;
    }
}
