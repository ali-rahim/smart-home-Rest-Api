using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using smart_home_Asp.net.Domain.Entities;
using SmartHoe_dbcontex;

namespace services
{
    public class HomeManager(SmartHome_dbcontex sdx, ILogger<Home> _logger)
    {
         public async Task<List<Home>> get_homeAsync()
         {
            return await sdx.Homes.OrderByDescending(c => c.Name).ThenByDescending(c => c.Id).AsNoTrackingWithIdentityResolution().ToListAsync();

        }

        public async Task<Home?> get_homeAsync(int id)
        {
            return await sdx.Homes.FirstOrDefaultAsync(ctx => ctx.Id == id);
        }



        public async Task<int> InsertdbhomeAsync(Home  home)
        {
            
                sdx.Homes.Add(home);
                await sdx.SaveChangesAsync();
            _logger.LogInformation(
           "Home created successfully. HomeId={home.Id}",
               home.Id);


            return home.Id;
        }




    }
}
