using smart_home_Asp.net.Domain.Devices.Base;


namespace smart_home_Asp.net.Domain.Entities
{
    public sealed class Room:Entity
    {
        public List<Device> Devices { get; set; }
        public int homeid { get; set; }
        
    }

}
