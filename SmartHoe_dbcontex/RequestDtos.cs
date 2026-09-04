namespace smart_home_Asp.net.Dtos
{
    public record CreateHomeRequest(string Name, int? Id);
    public record RoomRequest(string Name);
    public record UpdateHomeRequest(string Name);
    public record CreateDeviceRequest(string Name, string DeviceType, string ExternalId);
    public record UpdateDeviceRequest(string Name, string ExternalId);
    public record RecordSensorValueRequest(double Value);
    public record RecordDigitalStatusRequest(bool Status);
}