using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_class.Domain.Devices.Base
{
    
        public class DeviceReading
        {
            public int Id { get; private set; }
            public int DeviceId { get; private set; }
            public ReadingKind Kind { get; private set; }
            public double? NumericValue { get; private set; }
            public bool? BoolValue { get; private set; }
            public DateTime RecordedAt { get; private set; }

            private DeviceReading() { } // EF Core

            private DeviceReading(int deviceId, ReadingKind kind, double? numericValue, bool? boolValue)
            {
                DeviceId = deviceId;
                Kind = kind;
                NumericValue = numericValue;
                BoolValue = boolValue;
                RecordedAt = DateTime.UtcNow;
            }

            public static DeviceReading ForSensorValue(int deviceId, double value) =>
                new(deviceId, ReadingKind.SensorValue, value, null);

            public static DeviceReading ForDigitalStatus(int deviceId, bool status) =>
                new(deviceId, ReadingKind.DigitalStatus, null, status);

            public static DeviceReading ForSwitchCommand(int deviceId, bool isOn) =>
                new(deviceId, ReadingKind.SwitchCommand, null, isOn);
        }
    
}
