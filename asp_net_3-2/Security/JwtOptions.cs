using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.Security;

public sealed class JwtOptions
{
    [Required] public string Issuer { get; set; } = "";
    [Required] public string Audience { get; set; } = "";
    [Required, MinLength(32)] public string Key { get; set; } = "";
    [Range(1, 60)] public int LifetimeMinutes { get; set; } = 30;
}
