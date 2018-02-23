using Radarr.Core.Messaging.Commands.Interfaces;

namespace Radarr.Core.Messaging.Commands
{
    public class CleanupCommandMessagingService : IExecute<MessagingCleanupCommand>
    {
        private readonly ICommandQueueManager _commandQueueManager;

        public CleanupCommandMessagingService(ICommandQueueManager commandQueueManager)
        {
            _commandQueueManager = commandQueueManager;
        }

        public void Execute(MessagingCleanupCommand message)
        {
            _commandQueueManager.CleanCommands();
        }
    }
}
