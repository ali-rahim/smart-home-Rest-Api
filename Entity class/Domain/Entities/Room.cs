using smart_home_Asp.net.Domain.Devices.Base;

namespace smart_home_Asp.net.Domain.Entities
{
    public sealed class Room : Entity
    {
        public List<Device> Devices { get; set; } = new();
        public int homeid { get; set; }

        public Room(string name, int homeid) : base(name)
        {
            this.homeid = homeid;
        }

        private Room() { } // EF Core
    }
}