namespace smart_home_Asp.net.Exceptions
{
    public class EntityAlreadyExistsException : SmartHomeException
    {
        public string EntityId { get; }

        public EntityAlreadyExistsException(string entityId)
            : base($"Entity with Id '{entityId}' already exists.")
        {
            EntityId = entityId;
        }
    }
}
