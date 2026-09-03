using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smart_home_Asp.net.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHoe_dbcontex.Configuration
{
    internal class roomConfiguration : IEntityTypeConfiguration<Room>
    {
     
            public void Configure(EntityTypeBuilder<Room> builder)
            {
                builder.Property(c => c.Name).HasMaxLength(100);
                builder.HasMany(c => c.Devices).WithOne().HasForeignKey(c => c.Roomid).OnDelete(DeleteBehavior.Cascade);

        }

    }
}
