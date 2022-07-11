using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasker.Core.Logic.Statistics;
using Tasker.Core.Logic.Statistics.Responses;

namespace Tasker.Api.Controllers;

[Route("api/statistics")]
[Authorize(Policy = "AdminPolicy")]
public class StatisticsController : BaseApiController
{
    private readonly StatisticsService _statisticsService;

    public StatisticsController(StatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet]
    public async Task<ActionResult<List<GetStatisticsItemResponse>>> GetStatistics()
    {
        return Ok(await _statisticsService.GetStatisticsAsync());
    }
}
