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

        public async Task<Home?> UpdateHomeAsync(int id, string name)
        {
            var home = await sdx.Homes.FirstOrDefaultAsync(h => h.Id == id);
            if (home is null) return null;

            home.Rename(name);
            await sdx.SaveChangesAsync();
            return home;
        }
        public async Task<bool> DeleteHomeAsync(int id)
        {
            var home = await sdx.Homes.FirstOrDefaultAsync(h => h.Id == id);
            if (home is null) return false;

            sdx.Homes.Remove(home);
            await sdx.SaveChangesAsync();
            return true;
        }

    }
}
