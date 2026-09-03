using smart_home_Asp.net.Domain.Devices.ability_interfaces;
using smart_home_Asp.net.Domain.Devices.Base;

namespace smart_home_Asp.net.Domain.Devices
{
    public class door_sensor : Device, Idigital
    {
        public door_sensor(string name, int roomid, string externalId) : base(name, roomid, externalId) { }
        private door_sensor() { } // EF Core
        public bool Status { get ; set ; }

        public bool Get_Status()
        {
            return Status;
        }
    }
}
