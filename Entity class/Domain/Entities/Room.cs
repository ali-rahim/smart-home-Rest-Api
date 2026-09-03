using smart_home_Asp.net.Domain.Devices.Base;


namespace smart_home_Asp.net.Domain.Entities
{
    public class Room : CompositeEntity
    {
        public Room(string id) : base(id) { }
        protected override bool IsAllowedChild(Entity child) => child is Device;
    }

}
