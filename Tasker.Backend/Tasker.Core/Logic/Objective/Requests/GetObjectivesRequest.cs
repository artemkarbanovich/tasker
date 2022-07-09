namespace Tasker.Core.Logic.Objective.Requests;

public class GetObjectivesRequest
{
    private const int MinPageSize = 5;
    private const int MaxPageSize = 15;
    private int pageSize = 5;

    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => pageSize;
        set
        {
            if (value < MinPageSize)
                pageSize = MinPageSize;
            else if (value > MaxPageSize)
                pageSize = MaxPageSize;
            else
                pageSize = value;
        }
    }
}
