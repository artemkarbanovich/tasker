using System.Security.Claims;
using Tasker.Core.Interfaces.Repositories;
using Tasker.Core.Interfaces.Services;
using Tasker.Core.Logic.Token.Exceptions;
using Tasker.Core.Logic.Token.Responses;

namespace Tasker.Core.Logic.Token;

public class TokenService
{
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public TokenService(ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository)
    {
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = _tokenService.GetPrincipalFromAccessToken(accessToken);
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userRefreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(userId, refreshToken);

        if (userRefreshToken is null || userRefreshToken.ExpiryTime < DateTime.UtcNow)
            throw new RefreshTokenException("Unable to refresh token");

        userRefreshToken.UpdateToken();
        await _refreshTokenRepository.UpdateRefreshTokenAsync(userRefreshToken);

        return new RefreshTokenResponse(_tokenService.GenerateAccessToken(principal.Claims), userRefreshToken.Token);
    }

    public async Task RevokeTokenAsync(string userId, string refreshToken)
    {
        await _refreshTokenRepository.DeleteRefreshTokenAsync(userId, refreshToken);
    }
}
