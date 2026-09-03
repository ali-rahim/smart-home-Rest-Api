using smart_home_Asp.net.Domain.Entities;

namespace smart_home_Asp.net.Domain.Devices.Base
{
   public abstract class Device : Entity
{
    public Device(string id) : base(id) { }
    public bool IsOnline { get; protected set; }


   
}
}
