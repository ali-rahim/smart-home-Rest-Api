using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smart_home_Asp.net.Domain.Devices.ability_interfaces
{
    public interface Ianalog
    {
        public double value_sensor { get; set; }
        public double get_value();
    }
}
