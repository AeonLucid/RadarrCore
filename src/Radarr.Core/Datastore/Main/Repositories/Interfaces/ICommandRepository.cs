using System.Collections.Generic;
using Radarr.Core.Datastore.Base;
using Radarr.Core.Datastore.Main.Models;

namespace Radarr.Core.Datastore.Main.Repositories.Interfaces
{
    public interface ICommandRepository : IBasicRepository<CommandModel>
    {
        void Trim();

        void OrphanStarted();

        List<CommandModel> FindCommands(string name);

        List<CommandModel> FindQueuedOrStarted(string name);

        List<CommandModel> Queued();

        List<CommandModel> Started();

        void Start(CommandModel command);

        void End(CommandModel command);
    }
}
