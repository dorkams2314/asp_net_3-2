namespace asp_net_3_2.DTOs;

public sealed record PageResult<T>(IReadOnlyList<T> Items, int TotalCount);
