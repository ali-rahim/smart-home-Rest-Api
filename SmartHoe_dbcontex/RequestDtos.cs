namespace smart_home_Asp.net.Dtos
{
    public record CreateRoomRequest(string Name);
    public record CreateDeviceRequest(string Name, string DeviceType, string ExternalId);
}