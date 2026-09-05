using Entity_class.Domain.Devices.Base;
using Microsoft.EntityFrameworkCore;
using smart_home_Asp.net.Domain.Devices.ability_interfaces;
using SmartHoe_dbcontex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace services
{
    public class DeviceReadingManager(SmartHome_dbcontex sdx)
    {
        public async Task<DeviceReading?> RecordSensorValueAsync(int deviceId, double value)
        {
            var device = await sdx.Devices.FirstOrDefaultAsync(d => d.Id == deviceId);
            if (device is null) return null;

            if (device is not Ianalog analog)
                throw new InvalidOperationException($"Device {deviceId} is not an analog sensor.");

            analog.value_sensor = value;

            var reading = DeviceReading.ForSensorValue(deviceId, value);
            sdx.DeviceReadings.Add(reading);
            await sdx.SaveChangesAsync();

            return reading;
        }

        public async Task<DeviceReading?> RecordDigitalStatusAsync(int deviceId, bool status)
        {
            var device = await sdx.Devices.FirstOrDefaultAsync(d => d.Id == deviceId);
            if (device is null) return null;

            if (device is not Idigital digital)
                throw new InvalidOperationException($"Device {deviceId} is not a digital sensor.");

            digital.Status = status;

            var reading = DeviceReading.ForDigitalStatus(deviceId, status);
            sdx.DeviceReadings.Add(reading);
            await sdx.SaveChangesAsync();

            return reading;
        }

        public Task<DeviceReading?> GetLatestAsync(int deviceId, ReadingKind kind) =>
            sdx.DeviceReadings
                .Where(r => r.DeviceId == deviceId && r.Kind == kind)
                .OrderByDescending(r => r.RecordedAt)
                .FirstOrDefaultAsync();

        public Task<List<DeviceReading>> GetHistoryAsync(int deviceId, int count) =>
            sdx.DeviceReadings
                .Where(r => r.DeviceId == deviceId)
                .OrderByDescending(r => r.RecordedAt)
                .Take(count)
                .ToListAsync();
    }
}
