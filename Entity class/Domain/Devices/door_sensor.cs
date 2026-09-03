using smart_home_Asp.net.Domain.Devices.ability_interfaces;
using smart_home_Asp.net.Domain.Devices.Base;

namespace smart_home_Asp.net.Domain.Devices
{
    public class door_sensor : Device , Idigital
    {
        public door_sensor(string id) : base(id) { }
        public bool sensor_value { get; set; }=true;

        public bool get_value()
        {
            return sensor_value;
        }
    }
}
