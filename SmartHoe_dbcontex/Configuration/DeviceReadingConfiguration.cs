using Entity_class.Domain.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace SmartHoe_dbcontex.Configuration
{
    namespace SmartHoe_dbcontex.Configuration
    {
        internal class DeviceReadingConfiguration : IEntityTypeConfiguration<DeviceReading>
        {
            public void Configure(EntityTypeBuilder<DeviceReading> builder)
            {
                builder.ToTable("DeviceReadings");

                builder.Property(r => r.Kind)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                builder.HasIndex(r => new { r.DeviceId, r.RecordedAt });
            }
        }
    }
}
