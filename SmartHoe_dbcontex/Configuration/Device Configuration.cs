using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smart_home_Asp.net.Domain.Devices.Base;
using smart_home_Asp.net.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHoe_dbcontex.Configuration
{
    internal class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {

        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.Property(c => c.Name).HasMaxLength(100);
        }

    }

}
