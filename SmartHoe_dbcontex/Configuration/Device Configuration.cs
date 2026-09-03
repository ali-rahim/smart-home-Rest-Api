using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smart_home_Asp.net.Domain.Devices;
using smart_home_Asp.net.Domain.Devices.Base;

namespace SmartHoe_dbcontex.Configuration
{
    internal class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {


        public void Configure(EntityTypeBuilder<Device> builder)
        {

            builder.ToTable("Devices");

              builder.HasDiscriminator<string>("DeviceType")
                .HasValue<Fan>("Fan")
                .HasValue<Light>("Light")
                .HasValue<Rain_sensor>("RainSensor")
                .HasValue<SecurityAlarm>("SecurityAlarm")
                .HasValue<door_sensor>("DoorSensor");


            builder.Property(c => c.Name).HasMaxLength(100);

        }


    }

}
