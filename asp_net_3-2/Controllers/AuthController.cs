using asp_net_3_2.DTOs.Auth;
using asp_net_3_2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace asp_net_3_2.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController(IAccountService service) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterDto dto, CancellationToken cancellationToken) =>
        Ok(await service.RegisterAsync(dto, cancellationToken));

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<AuthResponse>> Login(LoginDto dto, CancellationToken cancellationToken)
    {
        var result = await service.LoginAsync(dto, cancellationToken);
        return result is null
            ? Problem(statusCode: 401, title: "Sign-in failed", detail: "Invalid credentials or inactive account.")
            : Ok(result);
    }
}
