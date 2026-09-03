using Microsoft.EntityFrameworkCore;
using smart_home_Asp.net.Domain.Entities;
using SmartHoe_dbcontex;

namespace services
{
    public class HomeManager(SmartHome_dbcontex sdx)
    {
         public async Task<List<Home>> get_homeAsync()
         {
            return await sdx.Homes.OrderByDescending(c => c.Name).ThenByDescending(c => c.Id).AsNoTrackingWithIdentityResolution().ToListAsync();

        }

        public async Task<Home?> get_homeAsync(int id)
        {
            return await sdx.Homes.FirstOrDefaultAsync(ctx => ctx.Id == id);
        }



        public async Task<int> InsertdbhomeAsync(Home  home )
        {
            try
            {
                sdx.Homes.Add(home);
                await sdx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error while saving room to database");
                throw;
            }

            return home.Id;
        }




    }
}
