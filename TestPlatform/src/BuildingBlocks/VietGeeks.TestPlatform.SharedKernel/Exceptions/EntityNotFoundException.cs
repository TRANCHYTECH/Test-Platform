namespace VietGeeks.TestPlatform.SharedKernel.Exceptions;

public class EntityNotFoundException: TestPlatformExecption
{
    public string EntityId { get; set; }

    public string EntityType { get; set; }

    public EntityNotFoundException(string entityId, string entityType)
    {
        EntityId = entityId;
        EntityType = entityType;
    }
}