using Tasker.Core.Entities;

namespace Tasker.Core.Interfaces.Repositories;

public interface IRefreshTokenRepository : IDisposable
{
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetRefreshTokenAsync(string userId, string token);
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
    Task DeleteRefreshTokenAsync(string userId, string token);
}
