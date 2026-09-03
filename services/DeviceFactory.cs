using smart_home_Asp.net.Domain.Devices;
using smart_home_Asp.net.Domain.Devices.Base;

namespace services
{
    public static class DeviceFactory
    {
        public static Device Create(DeviceType type, string name, int roomId, string externalId)
        {
            return type switch
            {
                DeviceType.Light => new Light(name, roomId, externalId),
                DeviceType.Fan => new Fan(name, roomId, externalId),
                DeviceType.SecurityAlarm => new SecurityAlarm(name, roomId, externalId),
                DeviceType.DoorSensor => new door_sensor(name, roomId, externalId),
                DeviceType.RainSensor => new Rain_sensor(name, roomId, externalId),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown device type.")
            };
        }
    }
}