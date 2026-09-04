using AutoMapper;
using smart_home_Asp.net.Domain.Devices.Base;
using smart_home_Asp.net.Domain.Entities;
using smart_home_Asp.net.Dtos;
using SmartHoe_dbcontex;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace smart_home_Asp.net.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Home, HomeResponse>();

            CreateMap<Room, RoomResponse>()
                .ForMember(dest => dest.HomeId, opt => opt.MapFrom(src => src.homeid));

            CreateMap<Device, DeviceResponse>()
                .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.Roomid));




        }
    }
}