using Radarr.Common.Messaging;
using Radarr.Core.Datastore.Databases.Main.Models;

namespace Radarr.Core.Messaging.Events
{
    public class CommandExecutedEvent : IEvent
    {
        public CommandExecutedEvent(CommandModel command)
        {
            Command = command;
        }

        public CommandModel Command { get; }
    }
}