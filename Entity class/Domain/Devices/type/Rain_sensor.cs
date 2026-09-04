using smart_home_Asp.net.Domain.Devices.ability_interfaces;
using smart_home_Asp.net.Domain.Devices.Base;

namespace Entity_class.Domain.Devices.type
{
    public class Rain_sensor : Device , Ianalog
    {

        public Rain_sensor(string name, int roomid, string externalId) : base(name, roomid, externalId) { }
        private Rain_sensor() { } // EF Core

        public double value_sensor { get; set; } = 25.85;


        public double get_value()
        {
            return value_sensor;
        }
    }
}
