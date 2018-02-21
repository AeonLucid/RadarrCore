using Microsoft.EntityFrameworkCore;

namespace Radarr.Database.Logs
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
