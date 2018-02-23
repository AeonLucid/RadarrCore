using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NLog;
using Radarr.Core.Datastore.Base;
using Radarr.Core.Datastore.Main.Models;
using Radarr.Core.Datastore.Main.Repositories.Interfaces;
using Radarr.Core.Messaging.Commands;
using Radarr.Core.Messaging.Events.Interfaces;

namespace Radarr.Core.Datastore.Main.Repositories
{
    public class CommandRepository : BasicRepository<CommandModel>, ICommandRepository
    {
        private readonly DbContextMain _database;

        public CommandRepository(DbContextMain database, IEventAggregator eventAggregator, ILogger logger) : base(database, eventAggregator, logger)
        {
            _database = database;
        }

        public void Trim()
        {
            var date = DateTime.UtcNow.AddDays(-1);

            Delete(c => c.EndedAt < date);
        }

        public void OrphanStarted()
        {
            _database.Database.ExecuteSqlCommand(@"UPDATE Commands SET Status = @Orphaned, EndedAt = @Ended WHERE Status = @Started", 
                new SqliteParameter("@Orphaned", CommandStatus.Orphaned), 
                new SqliteParameter("@Ended", CommandStatus.Started),
                new SqliteParameter("@Started", DateTime.UtcNow));
        }

        public List<CommandModel> FindCommands(string name)
        {
            return _database.Commands.Where(x => x.Name == name).ToList();
        }

        public List<CommandModel> FindQueuedOrStarted(string name)
        {
            return _database.Commands.Where(x => x.Name == name && (x.Status == CommandStatus.Queued || x.Status == CommandStatus.Started)).ToList();
        }

        public List<CommandModel> Queued()
        {
            return _database.Commands.Where(x => x.Status == CommandStatus.Queued).ToList();
        }

        public List<CommandModel> Started()
        {
            return _database.Commands.Where(x => x.Status == CommandStatus.Started).ToList();
        }

        public void Start(CommandModel command)
        {
            SetFields(command, c => c.StartedAt, c => c.Status);
        }

        public void End(CommandModel command)
        {
            SetFields(command, c => c.EndedAt, c => c.Status, c => c.Duration, c => c.Exception);
        }
    }
}
