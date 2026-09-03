namespace smart_home_Asp.net.Exceptions
{
    public class InvalidChildException : SmartHomeException
    {
        public string ParentId { get; }
        public string ChildId { get; }

        public InvalidChildException(string parentId, string childId)
            : base($"Entity '{childId}' is not a valid child of '{parentId}'.")
        {
            ParentId = parentId;
            ChildId = childId;
        }
    }
}
