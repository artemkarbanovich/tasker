using Tasker.Core.Interfaces.Repositories;
using Tasker.Core.Logic.FreeApi.Responses;

namespace Tasker.Core.Logic.FreeApi;

public class FreeApiService
{
    private readonly IFreeApiRepository _freeApiRepository;

    public FreeApiService(IFreeApiRepository freeApiRepository)
    {
        _freeApiRepository = freeApiRepository;
    }

    public async Task<List<GetFreeApisItemResponse>> GetFreeApisAsync()
    {
        return await _freeApiRepository.GetFreeApisAsync();
    }
}
