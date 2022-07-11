using Tasker.Core.Interfaces.Repositories;
using Tasker.Core.Logic.Statistics.Responses;

namespace Tasker.Core.Logic.Statistics;

public class StatisticsService
{
    private readonly IStatisticsRepository _statisticsRepository;

    public StatisticsService(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository;
    }

    public async Task<List<GetStatisticsItemResponse>> GetStatisticsAsync()
    {
        return await _statisticsRepository.GetStatisticsAsync();
    }
}
