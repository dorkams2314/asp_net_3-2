using Microsoft.AspNetCore.Authorization;
using asp_net_3_2.Security;
using asp_net_3_2.DTOs.Roles;
using asp_net_3_2.Services;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_3_2.Controllers;

[Authorize(Policy = Policies.Admin)]
[Route("api/roles")]
public sealed class RolesController(IService<RoleDto, RoleWriteDto, RoleWriteDto, PatchRoleDto> service)
    : CrudController<RoleDto, RoleWriteDto, RoleWriteDto, PatchRoleDto>(service)
{
}
