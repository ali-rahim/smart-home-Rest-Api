//using smart_home_Asp.net.Domain.Devices.Base;
//using smart_home_Asp.net.Domain.Entities;
//using global::smart_home_Asp.net.Exceptions;
//using Microsoft.Extensions.Logging;
//using SmartHoe_dbcontex;
//using Microsoft.Extensions.Logging.Abstractions;

//namespace smart_home_Asp.net.Services
//{
//    public class RoomManager
//    {
//        private readonly Dictionary<string, Room> _rooms =
//            new Dictionary<string, Room>();

//        private readonly ILogger<RoomManager> _logger;
//        private readonly SmartHome_dbcontex _SmartHome_dbcontex;
//        private NullLogger<RoomManager> instance;

//        public RoomManager(ILogger<RoomManager> logger, SmartHome_dbcontex SmartHome_dbcontex)
//        {
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//            _SmartHome_dbcontex=SmartHome_dbcontex ?? throw new ArgumentNullException(nameof(SmartHome_dbcontex));
//        }

//        public RoomManager(NullLogger<RoomManager> instance)
//        {
//            this.instance = instance;
//        }

//        public async Task<Room> CreateRoom(string id, Home home)
//        {
//            _logger.LogDebug(
//                "Creating room. RoomId={RoomId}",
//                id);

//            if (home == null)
//                throw new ArgumentNullException(nameof(home));

//            if (string.IsNullOrWhiteSpace(id))
//            {
//                _logger.LogWarning(
//                    "Attempt to create room with empty or invalid ID. RoomId={RoomId}",
//                    id);

//                throw new ArgumentException(
//                    "Id cannot be null or empty.",
//                    nameof(id));
//            }

//            if (_rooms.ContainsKey(id))
//            {
//                _logger.LogWarning(
//                    "Attempt to create room that already exists. RoomId={RoomId}",
//                    id);

//                throw new EntityAlreadyExistsException(id);
//            }

//            var room = new Room(id);
//            home.AddBelowEntity(room);
//            _rooms.Add(id, room);


//            await InsertdbRoomAsync(room);



//        _logger.LogInformation(
//                "Room created successfully. RoomId={RoomId}, TotalRooms={TotalRooms}",
//                id, _rooms.Count);

//            return room;
//        }

//        public async Task<string> InsertdbRoomAsync(Room room)
//        {
//         try{ 
//                    _SmartHome_dbcontex.Rooms.Add(room);
//                   await _SmartHome_dbcontex.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error while saving room to database");
//                throw;
//            }
//            return room.Id;
//        }

//        public void RemoveRoom(string id, Home home)
//        {
//            _logger.LogDebug(
//                "Removing room. RoomId={RoomId}",
//                id);

//            if (home == null)
//                throw new ArgumentNullException(nameof(home));

//            if (!_rooms.ContainsKey(id))
//            {
//                _logger.LogWarning(
//                    "Attempt to remove room that does not exist. RoomId={RoomId}",
//                    id);

//                throw new EntityNotFoundException(id);
//            }

//            home.RemoveBelowEntities(id);
//            _rooms.Remove(id);

//            _logger.LogInformation(
//                "Room removed successfully. RoomId={RoomId}, TotalRooms={TotalRooms}",
//                id, _rooms.Count);
//        }

//        public Room GetRoomById(string id)
//        {
//            _logger.LogDebug(
//                "Searching for room. RoomId={RoomId}",
//                id);

//            if (_rooms.TryGetValue(id, out var room))
//            {
//                _logger.LogInformation(
//                    "Room found successfully. RoomId={RoomId}",
//                    id);

//                return room;
//            }

//            _logger.LogWarning(
//                "Room not found. RoomId={RoomId}",
//                id);

//            throw new EntityNotFoundException(id);
//        }

//        public IReadOnlyList<Room> GetAllRooms()
//            => _rooms.Values.ToList().AsReadOnly();

//        public void AddDeviceToRoom(string roomId, Device device)
//        {
//            _logger.LogInformation(
//                "Adding device to room. DeviceId={DeviceId}, RoomId={RoomId}",
//                device.Id, roomId);

//            var room = GetRoomById(roomId);

//            room.AddBelowEntity(device);

//            _logger.LogInformation(
//                "Device added to room successfully. DeviceId={DeviceId}, RoomId={RoomId}",
//                device.Id, roomId);
//        }

//        public void RemoveDeviceFromRoom(string roomId, string deviceId)
//        {
//            _logger.LogInformation(
//                "Removing device from room. DeviceId={DeviceId}, RoomId={RoomId}",
//                deviceId, roomId);

//            var room = GetRoomById(roomId);

//            room.RemoveBelowEntities(deviceId);

//            _logger.LogInformation(
//                "Device removed from room successfully. DeviceId={DeviceId}, RoomId={RoomId}",
//                deviceId, roomId);
//        }

//        public void RemoveDeviceFromAllRooms(string deviceId)
//        {
//            _logger.LogDebug(
//                "Removing device from all rooms. DeviceId={DeviceId}",
//                deviceId);

//            foreach (var room in _rooms.Values)
//            {
//                if (room.GetBelowEntities().Any(e => e.Id == deviceId))
//                    room.RemoveBelowEntities(deviceId);
//            }
//        }

//        public IEnumerable<T> GetDevicesInRoomByCapability<T>(
//            string roomId) where T : class
//        {
//            _logger.LogDebug(
//                "Searching devices in room by capability. RoomId={RoomId}, Capability={Capability}",
//                roomId,
//                typeof(T).Name);

//            var room = GetRoomById(roomId);

//            return room.GetBelowEntities().OfType<T>();
//        }
//    }
//}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using smart_home_Asp.net.Domain.Entities;
using SmartHoe_dbcontex;

namespace services
{
    public class RoomManager(SmartHome_dbcontex sdx, ILogger<RoomManager> _logger)
    {
        public async Task<Room?> CreateRoomAsync(int homeId, string name)
        {
            var homeExists = await sdx.Homes.AnyAsync(h => h.Id == homeId);
            if (!homeExists) return null;

            var room = new Room(name, homeId);
            sdx.Rooms.Add(room);
            await sdx.SaveChangesAsync();

            _logger.LogInformation(
                "Room created successfully. RoomId={room.Id}",
                room.Id);
            return room;
        }

        public async Task<List<Room>> GetRoomsByHomeAsync(int homeId)
        {
            return await sdx.Rooms
                .Where(r => r.homeid == homeId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}