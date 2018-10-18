using Microsoft.EntityFrameworkCore;
using Radarr.Core.Datastore.Databases.Main.Models;

namespace Radarr.Core.Datastore.Databases.Main
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

        public DbSet<ScheduledTaskModel> ScheduledTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommandModel>(builder =>
            {
                builder.HasKey(model => model.Id);

                builder.Property(model => model.Name).IsRequired();
                builder.Property(model => model.Body).IsRequired();
                builder.Property(model => model.Priority).IsRequired();
                builder.Property(model => model.Status).IsRequired();
                builder.Property(model => model.QueuedAt).IsRequired();
                builder.Property(model => model.StartedAt).IsRequired(false);
                builder.Property(model => model.EndedAt).IsRequired(false);
                builder.Property(model => model.Duration).IsRequired(false);
                builder.Property(model => model.Exception).IsRequired(false);
                builder.Property(model => model.Trigger).IsRequired();
            });

            modelBuilder.Entity<ScheduledTaskModel>(builder =>
            {
                builder.HasKey(model => model.Id);

                builder.Property(model => model.TypeName).IsRequired();
                builder.Property(model => model.Interval).IsRequired();
                builder.Property(model => model.LastExecution).IsRequired();
            });
        }
    }
}
