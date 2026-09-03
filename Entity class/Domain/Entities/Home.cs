namespace smart_home_Asp.net.Domain.Entities
{
    public sealed class Home : Entity
    {
        public List<Room> Rooms { get; set; } = new();

        public Home(string name) : base(name) { }

        private Home() { } // EF Core
    }
}