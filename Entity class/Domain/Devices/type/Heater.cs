using smart_home_Asp.net.Domain.Devices.ability_interfaces;
using smart_home_Asp.net.Domain.Devices.Base;

namespace Entity_class.Domain.Devices.type
{
    public class Heater : Device, Iswitchable
    {
        public Heater(string name, int roomid, string externalId) : base(name, roomid, externalId) { }
        private Heater() { } // EF Core
        public bool IsOn { get; set; }=false;

        public void Turn_off()
        {
            if (IsOn == true)
            {
                IsOn = false;
            }
        }

        public void Turn_on()
        {
            if (IsOn == false)
            {
                IsOn = true;
            }
        }
    }
}
