using System;
using System.Collections.Generic;
using System.Threading;
using Radarr.Core.Datastore.Databases.Main.Models;

namespace Radarr.Core.Messaging.Commands.Interfaces
{
    public interface ICommandQueueManager
    {
        CommandModel Push<TCommand>(TCommand command, CommandPriority priority = CommandPriority.Normal, CommandTrigger trigger = CommandTrigger.Unspecified) where TCommand : Command;

        CommandModel Push(string commandName, DateTime? lastExecutionTime, CommandPriority priority = CommandPriority.Normal, CommandTrigger trigger = CommandTrigger.Unspecified);

        IEnumerable<CommandModel> Queue(CancellationToken cancellationToken);

        CommandModel Get(int id);

        List<CommandModel> GetStarted();

        void SetMessage(CommandModel command, string message);

        void Start(CommandModel command);

        void Complete(CommandModel command, string message);

        void Fail(CommandModel command, string message, Exception e);

        void Requeue();

        void CleanCommands();
    }
}
