using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Radarr.Core.Datastore.Databases.Main
{
    /// <summary>
    ///     Used by 'dotnet ef' tooling.
    /// </summary>
    public class DbContextMainDesign : IDesignTimeDbContextFactory<DbContextMain>
    {
        public DbContextMain CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContextMain>();
            optionsBuilder.UseSqlite("Data Source=main.db");

            return new DbContextMain(optionsBuilder.Options);
        }
    }
}
