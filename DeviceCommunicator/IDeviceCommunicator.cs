using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCommunicator
{
    public interface IDeviceCommunicator
    {
        public Task Esp_Turn_On(string ExternalId);
        public Task Esp_Turn_Off(string ExternalId);
        public Task Esp_get_sensor_value(string ExternalId);
        public Task Esp_get_sensor_status(string ExternalId);

    }
}
