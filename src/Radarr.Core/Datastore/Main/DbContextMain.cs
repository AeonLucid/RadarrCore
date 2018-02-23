using Microsoft.EntityFrameworkCore;
using Radarr.Core.Datastore.Main.Models;

namespace Radarr.Core.Datastore.Main
{
    public class DbContextMain : DbContext
    {
        protected DbContextMain()
        {

        }

        public DbContextMain(DbContextOptions options) : base(options)
        {

        }

        public DbSet<CommandModel> Commands { get; set; }
    }
}
