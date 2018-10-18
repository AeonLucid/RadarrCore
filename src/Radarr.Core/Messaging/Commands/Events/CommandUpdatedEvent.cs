using Radarr.Common.Messaging;
using Radarr.Core.Datastore.Databases.Main.Models;

namespace Radarr.Core.Messaging.Commands.Events
{
    public class CommandUpdatedEvent : IEvent
    {
        public CommandModel Command { get; set; }

        public CommandUpdatedEvent(CommandModel command)
        {
            Command = command;
        }
    }
}
