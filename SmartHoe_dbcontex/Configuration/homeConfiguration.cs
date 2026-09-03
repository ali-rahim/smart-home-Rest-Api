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
    internal class homeConfiguration : IEntityTypeConfiguration<Home>
    {

        public void Configure(EntityTypeBuilder<Home> builder)
        {
            builder.HasMany(c => c.Rooms).WithOne().HasForeignKey(c => c.homeid).OnDelete(DeleteBehavior.Cascade);

        }

    }
    
}
