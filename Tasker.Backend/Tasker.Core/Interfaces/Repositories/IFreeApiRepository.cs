using Tasker.Core.Entities;
using Tasker.Core.Logic.FreeApi.Responses;

namespace Tasker.Core.Interfaces.Repositories;

public interface IFreeApiRepository
{
    Task AddFreeApiAsync(FreeApi freeApi);
    Task<FreeApi?> GetFreeApiByIdAsync(string id);
    Task<List<GetFreeApisItemResponse>> GetFreeApisAsync();
    Task<bool> IsFreeApiExistByIdAsync(string id);
    Task<long> GetRowsCountAsync();
}
