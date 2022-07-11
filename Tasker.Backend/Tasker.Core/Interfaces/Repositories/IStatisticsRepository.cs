using Tasker.Core.Logic.Statistics.Responses;

namespace Tasker.Core.Interfaces.Repositories;

public interface IStatisticsRepository
{
    Task<List<GetStatisticsItemResponse>> GetStatisticsAsync();
}
