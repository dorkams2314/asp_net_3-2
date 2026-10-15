namespace asp_net_3_2.Domain;

public sealed class EntityNotFoundException(string message) : Exception(message);

public sealed class ConflictException(string message) : Exception(message);
