using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smart_home_Asp.net.Domain.Devices.ability_interfaces
{
    public interface Iswitchable
    {
        bool IsOn { get; set; }
        void Turn_on();
        void Turn_off();
    }
}
