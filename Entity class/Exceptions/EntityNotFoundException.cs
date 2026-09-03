namespace smart_home_Asp.net.Exceptions
{
    public class EntityNotFoundException : SmartHomeException
    {
        public string EntityId { get; }

        public EntityNotFoundException(string entityId)
            : base($"Entity with Id '{entityId}' was not found.")
        {
            EntityId = entityId;
        }
    }
}
