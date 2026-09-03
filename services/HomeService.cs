//using smart_home_Asp.net.Domain.Devices.ability_interfaces;
//using smart_home_Asp.net.Domain.Devices.Base;
//using smart_home_Asp.net.Domain.Entities;
//using smart_home_Asp.net.Exceptions;
//using Microsoft.Extensions.Logging;

//namespace smart_home_Asp.net.Services
//{
//    public class HomeService
//    {
//        private readonly Home _home;
//        private readonly DeviceManager _deviceManager;
//        private readonly RoomManager _roomManager;
//        private readonly ILogger<HomeService> _logger;

//        public HomeService(
//            DeviceManager deviceManager,
//            RoomManager roomManager,
//            Home home,
//            ILogger<HomeService> logger)
//        {
//            _home = home ?? throw new ArgumentNullException(nameof(home));
//            _deviceManager = deviceManager
//                ?? throw new ArgumentNullException(nameof(deviceManager));
//            _roomManager = roomManager
//                ?? throw new ArgumentNullException(nameof(roomManager));
//            _logger = logger
//                ?? throw new ArgumentNullException(nameof(logger));
//        }

//        // ---------- Room ----------

//        public async Task<Room>  AddRoom(string roomId)
//        {
//            _logger.LogInformation(
//                "Adding room to home. RoomId={RoomId}, HomeId={HomeId}",
//                roomId, _home.Id);

//           await _roomManager.CreateRoom(roomId, _home);

//            return _roomManager.GetRoomById(roomId);
//        }

//        public void RemoveRoom(string roomId)
//        {
//            _logger.LogInformation(
//                "Removing room from home. RoomId={RoomId}, HomeId={HomeId}",
//                roomId, _home.Id);

//            _roomManager.RemoveRoom(roomId, _home);
//        }

//        public Room GetRoomById(string roomId)
//            => _roomManager.GetRoomById(roomId);

//        public IReadOnlyList<Room> GetAllRooms()
//            => _roomManager.GetAllRooms();

//        public IEnumerable<T> GetDevicesInRoomByCapability<T>(
//            string roomId) where T : class
//            => _roomManager.GetDevicesInRoomByCapability<T>(roomId);

//        // ---------- Device ----------

//        public void Turn_on_off(Device device)
//        {
//            _logger.LogInformation(
//           "Device Turning_Off_On . TDeviceId={DeviceId}, HomeId={HomeId}", device.Id, _home.Id);
//            _deviceManager.Turn_on_oof(device);

//        }


//        public object get_Status(Device device)
//        {
//            _logger.LogInformation(
//            "Geting SensorValue. TDeviceId={DeviceId}, HomeId={HomeId}", device.Id , _home.Id);
//            return _deviceManager.GetSensorValue(device);
//        }

//        public Device GetDeviceById(string deviceId)
//            => _deviceManager.GetDeviceById(deviceId);

//        public Device CreateDevice(DeviceType type, string deviceId)
//        {
//            _logger.LogInformation(
//                "Creating device in home. DeviceId={DeviceId}, Type={DeviceType}, HomeId={HomeId}",
//                deviceId, type, _home.Id);

//            return _deviceManager.CreateDevice(type, deviceId);
//        }

//        public void AddDeviceToRoom(string deviceId, string roomId)
//        {
//            _logger.LogInformation(
//                "Adding device to room. DeviceId={DeviceId}, RoomId={RoomId}, HomeId={HomeId}",
//                deviceId, roomId, _home.Id);

//            var device = _deviceManager.GetDeviceById(deviceId);

//            _roomManager.AddDeviceToRoom(roomId, device);
//        }

//        public Device CreateDeviceInRoom(
//            DeviceType type,
//            string deviceId,
//            string roomId)
//        {
//            _logger.LogInformation(
//                "Creating device in room. DeviceId={DeviceId}, Type={DeviceType}, RoomId={RoomId}",
//                deviceId, type, roomId);

//            var device = _deviceManager.CreateDevice(type, deviceId);

//            try
//            {
//                _roomManager.AddDeviceToRoom(roomId, device);
//            }
//            catch (SmartHomeException)
//            {
//                _deviceManager.RemoveDevice(deviceId);
//                throw;
//            }

//            return device;
//        }

//        public void RemoveDeviceFromRoom(
//            string roomId,
//            string deviceId)
//        {
//            _logger.LogInformation(
//                "Removing device from room. DeviceId={DeviceId}, RoomId={RoomId}",
//                deviceId, roomId);

//            _roomManager.RemoveDeviceFromRoom(roomId, deviceId);
//        }

//        public void RemoveDeviceCompletely(string deviceId)
//        {
//            _logger.LogInformation(
//                "Removing device completely from home. DeviceId={DeviceId}, HomeId={HomeId}",
//                deviceId, _home.Id);

//            _deviceManager.RemoveDevice(deviceId);

//            _roomManager.RemoveDeviceFromAllRooms(deviceId);
//        }

//        public IReadOnlyList<Device> GetAllDevices()
//        {
//            _logger.LogDebug(
//                "Retrieving all devices from home. HomeId={HomeId}",
//                _home.Id);

//            return _deviceManager.GetAllDevices();
//        }

//        public IEnumerable<T> GetDevicesByCapability<T>()
//            where T : class
//            => _deviceManager.GetDevicesByCapability<T>();
//    }
//}