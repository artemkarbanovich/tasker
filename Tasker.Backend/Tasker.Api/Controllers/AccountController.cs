using Microsoft.AspNetCore.Mvc;
using Tasker.Api.Models.Account.Requests;
using Tasker.Core.Logic.Account;
using Tasker.Core.Logic.Account.Responses;

namespace Tasker.Api.Controllers;

[Route("api/account")]
public class AccountController : BaseApiController
{
    private readonly AccountService _accountService;

    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AccountResponse>> Register([FromBody] RegisterRequest request)
    {
        return Ok(await _accountService.RegisterAsync(request.Email, request.Username, request.Password));
    }

    [HttpPost("login-email")]
    public async Task<ActionResult> LoginByEmail([FromBody] LoginByEmailRequest request)
    {
        return Ok(await _accountService.LoginByEmailAsync(request.Email, request.Password));
    }

    [HttpPost("login-username")]
    public async Task<ActionResult> LoginByUsername([FromBody] LoginByUsernameRequest request)
    {
        return Ok(await _accountService.LoginByUsernameAsync(request.Username, request.Password));
    }
}
