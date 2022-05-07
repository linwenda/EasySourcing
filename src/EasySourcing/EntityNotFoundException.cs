namespace EasySourcing;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(Type entityType, object entityId) : base(entityId == null
        ? $"Entity not found. Entity type: {entityType.FullName}"
        : $"Entity not found. Entity type: {entityType.FullName}, id: {entityId}")
    {
    }
}