using System.Security.Claims;
using Tasker.Core.Entities;
using Tasker.Core.Enums;
using Tasker.Core.Exceptions;
using Tasker.Core.Interfaces.Repositories;
using Tasker.Core.Interfaces.Services;
using Tasker.Core.Logic.Account.Exceptions;
using Tasker.Core.Logic.Account.Responses;

namespace Tasker.Core.Logic.Account;

public class AccountService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AccountService(IUserRepository userRepository, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AccountResponse> RegisterAsync(string email, string username, string password)
    {
        if (await _userRepository.IsUserExistAsync(email, UserIdentifierType.Email))
            throw new EmailIsTakenException($"Email '{email}' is already taken");

        if (await _userRepository.IsUserExistAsync(username, UserIdentifierType.Username))
            throw new UsernameIsTakenException($"Username '{username}' is already taken");

        var user = new User(email, username, password);

        await _userRepository.AddUserAsync(user);

        return await ReturnAuthorizedUser(user);
    }

    public async Task<AccountResponse> LoginByEmailAsync(string email, string password)
    {
        var user = await _userRepository.GetUserAsync(email, UserIdentifierType.Email);

        if (user is null)
            throw new NotFoundException($"User with email '{email}' not found");

        if (!user.CheckPassword(password))
            throw new InvalidPasswordException("Invalid password");

        return await ReturnAuthorizedUser(user);
    }

    public async Task<AccountResponse> LoginByUsernameAsync(string username, string password)
    {
        var user = await _userRepository.GetUserAsync(username, UserIdentifierType.Username);

        if (user is null)
            throw new NotFoundException($"User with username '{username}' not found");

        if (!user.CheckPassword(password))
            throw new InvalidPasswordException("Invalid password");

        return await ReturnAuthorizedUser(user);
    }

    private async Task<AccountResponse> ReturnAuthorizedUser(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Role, user.Role)
        });

        var refreshToken = new RefreshToken(user.Id);

        await _refreshTokenRepository.AddRefreshTokenAsync(refreshToken);

        return new AccountResponse(user.Email, user.Username, accessToken, refreshToken.Token);
    }
}
