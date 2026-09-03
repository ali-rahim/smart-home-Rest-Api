using smart_home_Asp.net.Domain.Devices.ability_interfaces;
using smart_home_Asp.net.Domain.Devices.Base;

namespace smart_home_Asp.net.Domain.Devices
{
    public class Fan : Device, Iswitchable
    {
        public Fan(string id) : base(id) { }

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
