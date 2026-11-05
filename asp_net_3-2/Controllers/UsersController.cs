using System.ComponentModel.DataAnnotations;
using asp_net_3_2.DTOs.Auth;
using asp_net_3_2.DTOs.Users;
using asp_net_3_2.Security;
using asp_net_3_2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_3_2.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController(IAccountService service, IAuthorizationService authorization) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me(CancellationToken cancellationToken) =>
        Ok(await service.GetAsync(int.Parse(User.FindFirst("sub")!.Value), cancellationToken));

    [HttpGet]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetAll([Range(1, int.MaxValue / 100)] int page = 1,
        [Range(1, 100)] int pageSize = 10, CancellationToken cancellationToken = default) =>
        Ok(await service.GetPageAsync(page, pageSize, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> Get(int id, CancellationToken cancellationToken)
    {
        var profile = await service.GetAsync(id, cancellationToken);
        return (await authorization.AuthorizeAsync(User, profile, Policies.OwnerOrAdmin)).Succeeded
            ? Ok(profile) : Forbid();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProfileDto dto, CancellationToken cancellationToken)
    {
        var profile = await service.GetAsync(id, cancellationToken);
        if (!(await authorization.AuthorizeAsync(User, profile, Policies.OwnerOrAdmin)).Succeeded)
        {
            return Forbid();
        }

        await service.UpdateProfileAsync(id, dto, cancellationToken);
        return NoContent();
    }
}
