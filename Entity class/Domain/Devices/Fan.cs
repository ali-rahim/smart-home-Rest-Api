using smart_home_Asp.net.Domain.Devices.ability_interfaces;
using smart_home_Asp.net.Domain.Devices.Base;

namespace smart_home_Asp.net.Domain.Devices
{
    public class Fan : Device, Iswitchable
    {
        public Fan(string name, int roomid, string externalId) : base(name, roomid, externalId) { }
        private Fan() { } // EF Core
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
