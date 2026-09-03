using Microsoft.EntityFrameworkCore;
using smart_home_Asp.net.Domain.Devices.Base;
using smart_home_Asp.net.Domain.Entities;

namespace SmartHoe_dbcontex
{
    public class SmartHome_dbcontex : DbContext
    {
        public SmartHome_dbcontex(
       DbContextOptions<SmartHome_dbcontex> options)
       : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            base.OnModelCreating(modelBuilder);

        }


        public DbSet<Home> Homes { get; set; }
        public DbSet<Room> Rooms { get ;  set;}
        public DbSet<Device> Devices { get; set; }





    }
}
