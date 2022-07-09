using Tasker.Core.Entities;
using Tasker.Core.Logic.Objective.Requests;
using Tasker.Core.Logic.Objective.Responses;

namespace Tasker.Core.Interfaces.Repositories;

public interface IObjectiveRepository
{
    Task AddObjectiveAsync(Objective objective);
    Task<GetObjectivesResponse> GetObjectivesAsync(string userId, GetObjectivesRequest query);
    Task<Objective?> GetObjectiveByIdAsync(string objectiveId);
    Task DeleteObjectiveByIdAsync(string userId, string objectiveId);
    Task UpdateObjectiveAsync(Objective objective);
}
