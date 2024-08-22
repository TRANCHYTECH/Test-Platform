namespace VietGeeks.TestPlatform.SharedKernel.Exceptions;

public class EntityNotFoundException(string entityId, string entityType) : TestPlatformException
{
    public string EntityId { get; set; } = entityId;

    public string EntityType { get; set; } = entityType;
}