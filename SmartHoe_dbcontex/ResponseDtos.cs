using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHoe_dbcontex
{
    public class HomeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class RoomResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int HomeId { get; set; }
    }

    public class DeviceResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string ExternalId { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public bool IsOnline { get; set; }
    }
    public class DeviceReadingResponse
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string Kind { get; set; } = string.Empty;
        public double? NumericValue { get; set; }
        public bool? BoolValue { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
