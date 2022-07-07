using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasker.Core.Logic.FreeApi;
using Tasker.Core.Logic.FreeApi.Responses;

namespace Tasker.Api.Controllers;

[Route("api/free-apis")]
public class FreeApiController : BaseApiController
{
    private readonly FreeApiService _freeApiService;

    public FreeApiController(FreeApiService freeApiService)
    {
        _freeApiService = freeApiService;
    }

    [HttpGet]
    [Authorize(Policy = "UserPolicy")]
    public async Task<ActionResult<List<GetFreeApisItemResponse>>> GetFreeApis()
    {
        return Ok(await _freeApiService.GetFreeApisAsync());
    }
}
