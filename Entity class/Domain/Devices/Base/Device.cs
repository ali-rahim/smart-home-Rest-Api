using smart_home_Asp.net.Domain.Entities;

namespace smart_home_Asp.net.Domain.Devices.Base
{
   public abstract class Device : Entity
{
        public bool IsOnline { get; protected set; }
        public int Roomid { get; set; }
        public string ExternalId { get; set; }


    }
}
