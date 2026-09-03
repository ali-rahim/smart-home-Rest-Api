
namespace smart_home_Asp.net.Domain.Entities
{

    public abstract class Entity
    {
         public  string Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        
    }
}