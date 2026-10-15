namespace asp_net_3_2.DTOs.Roles;

public sealed class RoleDto : IResourceDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public IReadOnlyList<UserSummaryDto> Users { get; set; } = [];
}
