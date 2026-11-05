using Microsoft.AspNetCore.Authorization;
using asp_net_3_2.DTOs.Users;

namespace asp_net_3_2.Security;

public static class Policies
{
    public const string Admin = "Admin";
    public const string OwnerOrAdmin = "OwnerOrAdmin";
}

public sealed class OwnerRequirement : IAuthorizationRequirement;

public sealed class ProfileAuthorizationHandler : AuthorizationHandler<OwnerRequirement, UserDto>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OwnerRequirement requirement, UserDto resource)
    {
        if (context.User.IsInRole("Admin") || context.User.FindFirst("sub")?.Value == resource.Id.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
