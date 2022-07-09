namespace Tasker.Core.Logic.Objective.Responses;

public class GetObjectivesResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public List<GetObjectivesItems> Objectives { get; set; } = new List<GetObjectivesItems>();
}
