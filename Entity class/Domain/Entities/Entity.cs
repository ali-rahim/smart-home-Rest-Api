
namespace smart_home_Asp.net.Domain.Entities
{

    public abstract class Entity
    {
        public int Id { get; protected set; }
        public string Name { get; protected set; } = string.Empty;

        protected Entity(string name)
        {
            Name = name;
        }

        // برای EF Core لازمه
        protected Entity() { }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.", nameof(name));

            Name = name;
        }
    }
}
