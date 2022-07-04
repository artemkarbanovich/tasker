using Tasker.Core.Entities;
using Tasker.Core.Enums;

namespace Tasker.Core.Interfaces.Repositories;

public interface IUserRepository : IDisposable
{
    Task<bool> IsUserExistAsync(string identifier, UserIdentifierType userIdentifierType);
    Task AddUserAsync(User user);
    Task<User?> GetUserAsync(string identifier, UserIdentifierType userIdentifierType);
}
