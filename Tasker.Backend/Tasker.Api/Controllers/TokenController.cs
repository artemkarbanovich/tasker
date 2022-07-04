using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasker.Api.Extensions;
using Tasker.Api.Models.Token.Requests;
using Tasker.Core.Logic.Token;
using Tasker.Core.Logic.Token.Responses;

namespace Tasker.Api.Controllers;

[Route("api/token")]
public class TokenController : BaseApiController
{
    private readonly TokenService _tokenService;

    public TokenController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        return Ok(await _tokenService.RefreshTokenAsync(request.AccessToken, request.RefreshToken));
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<ActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        await _tokenService.RevokeTokenAsync(User.GetId(), request.RefreshToken);
        return NoContent();
    }
}
