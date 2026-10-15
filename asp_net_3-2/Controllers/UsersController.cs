using asp_net_3_2.DTOs.Users;
using asp_net_3_2.Services;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_3_2.Controllers;

[Route("api/users")]
public sealed class UsersController(IService<UserDto, UserWriteDto, UserWriteDto, PatchUserDto> service)
    : CrudController<UserDto, UserWriteDto, UserWriteDto, PatchUserDto>(service)
{
}
