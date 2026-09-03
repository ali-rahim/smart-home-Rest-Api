using smart_home_Asp.net.Domain.Devices.ability_interfaces;
using smart_home_Asp.net.Domain.Devices.Base;

namespace smart_home_Asp.net.Domain.Devices
{
    public class Rain_sensor : Device , Ianalog
    {
        public Rain_sensor(string id) : base(id) { }

        public double value_sensor { get; set; } = 25.85;


        public double get_value()
        {
            return value_sensor;
        }
    }
}
