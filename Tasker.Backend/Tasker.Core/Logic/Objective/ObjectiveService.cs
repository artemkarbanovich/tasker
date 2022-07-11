using Tasker.Core.Interfaces.Repositories;
using Tasker.Core.Exceptions;
using Tasker.Core.Enums;
using Tasker.Core.Logic.Objective.Responses;
using Tasker.Core.Logic.Objective.Requests;
using Tasker.Core.Interfaces.Schedulers;
using Tasker.Core.DTOs;

namespace Tasker.Core.Logic.Objective;

public class ObjectiveService
{
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly IFreeApiRepository _freeApiRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMailingScheduler _mailingScheduler;

    public ObjectiveService(
        IObjectiveRepository objectiveRepository,
        IFreeApiRepository freeApiRepository,
        IUserRepository userRepository,
        IMailingScheduler mailingScheduler)
    {
        _objectiveRepository = objectiveRepository;
        _freeApiRepository = freeApiRepository;
        _userRepository = userRepository;
        _mailingScheduler = mailingScheduler;
    }

    public async Task CreateObjectiveAsync(string userId, string name, string description, DateTime startAt,
        int periodInMinutes, string freeApiId, string? query)
    {
        var user = await _userRepository.GetUserAsync(userId, UserIdentifierType.Id);

        if (user is null)
            throw new NotFoundException($"User with id {userId} not found");

        var freeApi = await _freeApiRepository.GetFreeApiByIdAsync(freeApiId);

        if (freeApi is null)
            throw new NotFoundException($"Free api with id {freeApiId} not found");

        if (freeApi.IsQueryRequired && string.IsNullOrWhiteSpace(query))
            throw new DefaultException("Query string cannot be null or empty");

        var objective = new Entities.Objective(
            userId: userId,
            name: name,
            description: description,
            startAt: startAt,
            periodInMinutes: periodInMinutes,
            freeApiId: freeApiId,
            query: query);

        await _objectiveRepository.AddObjectiveAsync(objective);

        var mailingJobDTO = new MailingJobDTO(
            UserId: user.Id,
            UserEmail: user.Email,
            FreeApiUrl: freeApi.ApiUrl,
            FreeApiRapidApiHost: freeApi.RapidApiHost,
            FreeApiQueryKey: freeApi.QueryKey,
            FreeApiRequiredQueryPrams: freeApi.RequiredQueryParams,
            ObjectiveId: objective.Id,
            ObjectiveName: objective.Name,
            ObjectiveStartAt: objective.StartAt,
            ObjectivePeriodInMinutes: objective.PeriodInMinutes,
            ObjectiveQuery: objective.Query);

        await _mailingScheduler.StartMailingJobAsync(mailingJobDTO);
    }

    public async Task<GetObjectivesResponse> GetObjectivesAsync(string userId, GetObjectivesRequest query)
    {
        if (!await _userRepository.IsUserExistAsync(userId, UserIdentifierType.Id))
            throw new NotFoundException($"User with id {userId} not found");

        return await _objectiveRepository.GetObjectivesAsync(userId, query);
    }

    public async Task<Entities.Objective> GetObjectiveByIdAsync(string userId, string objectiveId)
    {
        if (!await _userRepository.IsUserExistAsync(userId, UserIdentifierType.Id))
            throw new NotFoundException($"User with id {userId} not found");

        var objective = await _objectiveRepository.GetObjectiveByIdAsync(objectiveId);

        if (objective is null || objective.IsDeleted || objective.UserId != userId)
            throw new NotFoundException($"Objective with id {objectiveId} not found");

        return objective;
    }

    public async Task DeleteObjectiveAsync(string userId, string objectiveId)
    {
        if (!await _userRepository.IsUserExistAsync(userId, UserIdentifierType.Id))
            throw new NotFoundException($"User with id {userId} not found");

        var objective = await _objectiveRepository.GetObjectiveByIdAsync(objectiveId);

        if (objective is null || objective.IsDeleted || objective.UserId != userId)
            throw new NotFoundException($"Objective with id {objectiveId} not found");

        await _objectiveRepository.DeleteObjectiveByIdAsync(userId, objective.Id);

        await _mailingScheduler.StopMailingJobAsync(objective.Id);
    }

    public async Task UpdateObjectiveAsync(
        string userId, string objectiveId, string name, string description, DateTime startAt, int periodInMinutes, string freeApiId, string? query)
    {
        var user = await _userRepository.GetUserAsync(userId, UserIdentifierType.Id);

        if (user is null)
            throw new NotFoundException($"User with id {userId} not found");

        var objective = await _objectiveRepository.GetObjectiveByIdAsync(objectiveId);

        if (objective is null || objective.IsDeleted || objective.UserId != userId)
            throw new NotFoundException($"Objective with id {objectiveId} not found");

        var freeApi = await _freeApiRepository.GetFreeApiByIdAsync(freeApiId);

        if (freeApi is null)
            throw new NotFoundException($"Free api with id {freeApiId} not found");

        if (freeApi.IsQueryRequired && string.IsNullOrWhiteSpace(query))
            throw new DefaultException("Query string cannot be null or empty");

        objective.UpdateObjective(name, description, startAt, periodInMinutes, freeApiId, query);

        await _objectiveRepository.UpdateObjectiveAsync(objective);

        await _mailingScheduler.StopMailingJobAsync(objective.Id);

        var mailingJobDTO = new MailingJobDTO(
            UserId: user.Id,
            UserEmail: user.Email,
            FreeApiUrl: freeApi.ApiUrl,
            FreeApiRapidApiHost: freeApi.RapidApiHost,
            FreeApiQueryKey: freeApi.QueryKey,
            FreeApiRequiredQueryPrams: freeApi.RequiredQueryParams,
            ObjectiveId: objective.Id,
            ObjectiveName: objective.Name,
            ObjectiveStartAt: objective.StartAt,
            ObjectivePeriodInMinutes: objective.PeriodInMinutes,
            ObjectiveQuery: objective.Query);

        await _mailingScheduler.StartMailingJobAsync(mailingJobDTO);
    }
}
