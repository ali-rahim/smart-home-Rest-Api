using Entity_class.Domain.Devices;
using Microsoft.EntityFrameworkCore;
using smart_home_Asp.net.Domain.Devices.ability_interfaces;
using smart_home_Asp.net.Domain.Devices.Base;
using SmartHoe_dbcontex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace services
{
    public class DeviceCommandManager(SmartHome_dbcontex sdx)
    {
        public Task<Device?> TurnOnAsync(int deviceId) => SetSwitchAsync(deviceId, turnOn: true);

        public Task<Device?> TurnOffAsync(int deviceId) => SetSwitchAsync(deviceId, turnOn: false);

        private async Task<Device?> SetSwitchAsync(int deviceId, bool turnOn)
        {
            var device = await sdx.Devices.FirstOrDefaultAsync(d => d.Id == deviceId);
            if (device is null) return null;

            if (device is not Iswitchable switchable)
                throw new InvalidOperationException($"Device {deviceId} does not support switching.");

            if (turnOn) switchable.Turn_on();
            else switchable.Turn_off();

            sdx.DeviceReadings.Add(DeviceReading.ForSwitchCommand(deviceId, switchable.IsOn));
            await sdx.SaveChangesAsync();

            return device;
        }
    }
}
