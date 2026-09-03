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
            builder.Property(c => c.Name).HasMaxLength(100);
        }

    }
    
}
