using smart_home_Asp.net.Domain.Devices.ability_interfaces;
using smart_home_Asp.net.Domain.Devices.Base;

namespace smart_home_Asp.net.Domain.Devices
{
    public class door_sensor : Device, Idigital
    {
        public bool Status { get ; set ; }

        public bool Get_Status()
        {
            return Status;
        }
    }
}
