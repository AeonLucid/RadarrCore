using Microsoft.EntityFrameworkCore;

namespace Radarr.Database.Main
{
    public class DbContextMain : DbContext
    {
        protected DbContextMain()
        {

        }

        public DbContextMain(DbContextOptions options) : base(options)
        {

        }
    }
}
