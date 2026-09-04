//using smart_home_Asp.net.Domain.Devices.ability_interfaces;
//using smart_home_Asp.net.Domain.Devices.Base;
//using smart_home_Asp.net.Exceptions;
//using Microsoft.Extensions.Logging;

//namespace smart_home_Asp.net.Services
//{
//    public class DeviceManager
//    {
//        private readonly Dictionary<string, Device> _devices = new Dictionary<string, Device>();
//        private readonly ILogger<DeviceManager> _logger;

//        public DeviceManager(ILogger<DeviceManager> logger)
//        {
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        public Device CreateDevice(DeviceType type, string id)
//        {

//                _logger.LogDebug(
//                    "Creating device. DeviceId={DeviceId}, Type={DeviceType}",
//                    id, type);

//                if (string.IsNullOrWhiteSpace(id))
//                {
//                    _logger.LogWarning(
//                        "Attempt to create device with empty or invalid ID. DeviceId={DeviceId}, Type={DeviceType}",
//                        id, type);

//                    throw new ArgumentException(
//                        "Id cannot be null or empty.",
//                        nameof(id));
//                }

//                if (_devices.ContainsKey(id))
//                {
//                    _logger.LogWarning(
//                        "Attempt to create device that already exists. DeviceId={DeviceId}, Type={DeviceType}",
//                        id, type);

//                    throw new EntityAlreadyExistsException(id);
//                }

//                var device = DeviceFactory.Create(type, id);
//                _devices.Add(id, device);

//                _logger.LogInformation(
//                    "Device created successfully. DeviceId={DeviceId}, Type={DeviceType}, TotalDevices={TotalDevices}",
//                    id, type, _devices.Count);

//            return device;

//        }

//        public void RemoveDevice(string id)
//        {
//            _logger.LogDebug(
//                "Removing device. DeviceId={DeviceId}",
//                id);

//            if (!_devices.ContainsKey(id))
//            {
//                _logger.LogWarning(
//                    "Attempt to remove device that does not exist. DeviceId={DeviceId}",
//                    id);

//                throw new EntityNotFoundException(id);
//            }

//            _devices.Remove(id);

//            _logger.LogInformation(
//                "Device removed successfully. DeviceId={DeviceId}, TotalDevices={TotalDevices}",
//                id, _devices.Count);
//        }

//        public Device GetDeviceById(string id)
//        {
//            _logger.LogDebug(
//                "Searching for device. DeviceId={DeviceId}",
//                id);

//            if (_devices.TryGetValue(id, out var device))
//            {
//                _logger.LogInformation(
//                    "Device found successfully. DeviceId={DeviceId}",
//                    id);

//                return device;
//            }

//            _logger.LogWarning(
//                "Device not found. DeviceId={DeviceId}",
//                id);

//            throw new EntityNotFoundException(id);
//        }

//        public IReadOnlyList<Device> GetAllDevices()
//        {
//            _logger.LogDebug(
//                "Retrieving all devices. TotalDevices={TotalDevices}",
//                _devices.Count);

//            return _devices.Values.ToList().AsReadOnly();
//        }

//        public IEnumerable<T> GetDevicesByCapability<T>() where T : class
//        {
//            _logger.LogDebug(
//                "Searching devices by capability. Capability={Capability}",
//                typeof(T).Name);

//            return _devices.Values.OfType<T>();
//        }


//        public void Turn_on_oof(Device device)
//        {

//            var switchable = (Iswitchable)device;

//            if (switchable.IsOn == true)
//            {
//                switchable.Turn_off();

//                _logger.LogInformation(
//                 "Device Turn_Off successfully. DeviceId={DeviceId}",
//                 device.Id);

//            }

//            else 
//            {
//                switchable.Turn_on();

//                _logger.LogInformation(
//                  "Device Turn_On successfully. DeviceId={DeviceId}",
//                  device.Id);
//            }



//        }

//        public object GetSensorValue(Device device)
//        {



//            if (device is Idigital digital)
//            {
//                _logger.LogDebug(
//            "Get SensorValue. TDeviceId={DeviceId}", device.Id);

//                return digital.get_Status();
//            }


//            if (device is Ianalog analog) 
//            {
//                _logger.LogDebug(
//             "Get SensorValue. TDeviceId={DeviceId}", device.Id);
//                return analog.get_value();

//            }
//            throw new InvalidOperationException("This device is not a sensor (neither digital nor analog).");
//        }

//    }
//}


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using smart_home_Asp.net.Domain.Devices.Base;
using smart_home_Asp.net.Exceptions;
using SmartHoe_dbcontex;

namespace services
{
    public class DeviceManager(SmartHome_dbcontex sdx, ILogger<DeviceManager> _logger)
    {
        public async Task<Device?> CreateDeviceAsync(int roomId, DeviceType type, string name, string externalId)
        {
            var roomExists = await sdx.Rooms.AnyAsync(r => r.Id == roomId);
            if (!roomExists) return null;

            var device = DeviceFactory.Create(type, name, roomId, externalId);
            sdx.Devices.Add(device);

            try
            {
                await sdx.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new EntityAlreadyExistsException(externalId);
            }

            _logger.LogInformation(
              "Device created successfully. DeviceId={device.Id}",
             device.Id);
            return device;
        }

        public async Task<List<Device>> GetDevicesByRoomAsync(int roomId)
        {
            return await sdx.Devices
                .Where(d => d.Roomid == roomId)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Device?> UpdateDeviceAsync(int roomId, int deviceId, string name, string externalId)
        {
            var device = await sdx.Devices.FirstOrDefaultAsync(d => d.Id == deviceId && d.Roomid == roomId);
            if (device is null) return null;

            var externalIdTakenByAnother = await sdx.Devices
                .AnyAsync(d => d.ExternalId == externalId && d.Id != deviceId);
            if (externalIdTakenByAnother)
                throw new EntityAlreadyExistsException(externalId);

            device.Rename(name);
            device.ExternalId = externalId;

            try
            {
                await sdx.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new EntityAlreadyExistsException(externalId);
            }

            return device;
        }
    }
}