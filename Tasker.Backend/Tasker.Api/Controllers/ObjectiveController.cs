using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasker.Api.Extensions;
using Tasker.Api.Models.Objective.Requests;
using Tasker.Core.Logic.Objective;
using Tasker.Core.Logic.Objective.Responses;

namespace Tasker.Api.Controllers;

[Route("api/objectives")]
[Authorize(Policy = "UserPolicy")]
public class ObjectiveController : BaseApiController
{
    private readonly ObjectiveService _objectiveService;
    private readonly IMapper _mapper;

    public ObjectiveController(ObjectiveService objectiveService, IMapper mapper)
    {
        _objectiveService = objectiveService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult> CreateObjective([FromBody] CreateObjectiveRequest request)
    {
        await _objectiveService.CreateObjectiveAsync(User.GetId(), request.Name, request.Description,
            request.StartAt, request.PeriodInMinutes, request.FreeApiId, request.Query);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<List<GetObjectivesItemResponse>>> GetObjectives()
    {
        return Ok(await _objectiveService.GetObjectivesAsync(User.GetId()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetObjectiveResponse>> GetObjectiveById([FromRoute] string id)
    {
        var objective = await _objectiveService.GetObjectiveByIdAsync(User.GetId(), id);
        return Ok(_mapper.Map<GetObjectiveResponse>(objective));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteObjectiveById([FromRoute] string id)
    {
        await _objectiveService.DeleteObjectiveAsync(User.GetId(), id);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateObjective([FromBody] UpdateObjectiveRequest request)
    {
        await _objectiveService.UpdateObjectiveAsync(User.GetId(), request.Id, request.Name, request.Description,
            request.StartAt, request.PeriodInMinutes, request.FreeApiId, request.Query);
        return NoContent();
    }
}
