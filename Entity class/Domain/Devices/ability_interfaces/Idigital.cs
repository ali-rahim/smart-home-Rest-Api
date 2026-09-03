using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smart_home_Asp.net.Domain.Devices.ability_interfaces
{ 
    public interface Idigital
    {
        public bool sensor_value { get; set; }
        public bool get_value();
        
    }
}
