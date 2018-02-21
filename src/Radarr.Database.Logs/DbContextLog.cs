using Microsoft.EntityFrameworkCore;

namespace Radarr.Database.Log
{
    public class DbContextLog : DbContext
    {
        protected DbContextLog()
        {

        }

        public DbContextLog(DbContextOptions options) : base(options)
        {

        }
    }
}
