//using smart_home_Asp.net.Domain.Devices;
//using smart_home_Asp.net.Domain.Devices.Base;

//namespace smart_home_Asp.net.Services
//{
//    public static class DeviceFactory
//    {
//        public static Device Create(DeviceType type, string id)
//        {
//            return type switch
//            {
//                DeviceType.Light => new Light(id),
//                DeviceType.fan => new Fan(id),
//                DeviceType.dozdgir => new SecurityAlarm(id),
//                DeviceType.door_sensor => new door_sensor(id),
//                DeviceType.rain_sensor => new Rain_sensor(id),
//                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown device type.")
//            };
//        }
//    }
//}
