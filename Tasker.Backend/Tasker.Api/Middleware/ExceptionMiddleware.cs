using Microsoft.IdentityModel.Tokens;
using System.Net;
using Tasker.Api.Models;
using Tasker.Core.Exceptions;
using Tasker.Core.Logic.Account.Exceptions;
using Tasker.Core.Logic.Token.Exceptions;

namespace Tasker.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            _logger.LogInformation(ex.ToString());

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = GetStatusCode(ex);

            await context.Response.WriteAsync(
                new ErrorModel
                {
                    StatusCode = context.Response.StatusCode,
                    Message = context.Response.StatusCode == (int)HttpStatusCode.InternalServerError ? "Internal server error" : ex.Message
                }.ToString()
            );
        }
    }

    private static int GetStatusCode(Exception ex)
    {
        #region Built-in exceptions
        if (ex is SecurityTokenException) return (int)HttpStatusCode.Unauthorized;
        #endregion

        #region Core general exceptions
        else if (ex is DefaultException) return (int)HttpStatusCode.BadRequest;
        else if (ex is NotFoundException) return (int)HttpStatusCode.NotFound;
        #endregion

        #region Account exceptions
        else if (ex is EmailIsTakenException) return (int)HttpStatusCode.BadRequest;
        else if (ex is UsernameIsTakenException) return (int)HttpStatusCode.BadRequest;
        else if (ex is InvalidPasswordException) return (int)HttpStatusCode.Unauthorized;
        #endregion

        #region Token exceptions
        else if (ex is RefreshTokenException) return (int)HttpStatusCode.Unauthorized;
        #endregion

        else return (int)HttpStatusCode.InternalServerError;
    }
}
