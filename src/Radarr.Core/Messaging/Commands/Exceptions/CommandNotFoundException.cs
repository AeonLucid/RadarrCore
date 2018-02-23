using Radarr.Common.Exceptions;

namespace Radarr.Core.Messaging.Commands.Exceptions
{
    public class CommandNotFoundException : RadarrException
    {
        public CommandNotFoundException(string contract) : base("Couldn't find command " + contract)
        {
        }

    }
}
