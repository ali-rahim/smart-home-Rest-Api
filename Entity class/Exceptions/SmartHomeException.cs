namespace smart_home_Asp.net.Exceptions
{
    
        public abstract class SmartHomeException : Exception
        {
            protected SmartHomeException(string message) : base(message) { }

            protected SmartHomeException(string message, Exception innerException) : base(message, innerException) { }
        }
    
}
