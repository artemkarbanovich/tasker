using Tasker.Core.Interfaces.Repositories;
using Tasker.Core.Exceptions;
using Tasker.Core.Enums;
using Tasker.Core.Logic.Objective.Responses;
using Tasker.Core.Logic.Objective.Requests;

namespace Tasker.Core.Logic.Objective;

public class ObjectiveService
{
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly IFreeApiRepository _freeApiRepository;
    private readonly IUserRepository _userRepository;

    public ObjectiveService(
        IObjectiveRepository objectiveRepository,
        IFreeApiRepository freeApiRepository,
        IUserRepository userRepository)
    {
        _objectiveRepository = objectiveRepository;
        _freeApiRepository = freeApiRepository;
        _userRepository = userRepository;
    }

    public async Task CreateObjectiveAsync(string userId, string name, string description, DateTime startAt,
        int periodInMinutes, string freeApiId, string? query)
    {
        if (!await _userRepository.IsUserExistAsync(userId, UserIdentifierType.Id))
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

        // Add objective to CRON
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

        await _objectiveRepository.DeleteObjectiveByIdAsync(userId, objectiveId);

        // Delete objective from CRON
    }

    public async Task UpdateObjectiveAsync(
        string userId, string objectiveId, string name, string description, DateTime startAt, int periodInMinutes, string freeApiId, string? query)
    {
        if (!await _userRepository.IsUserExistAsync(userId, UserIdentifierType.Id))
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

        // Update objective in CRON
    }
}
