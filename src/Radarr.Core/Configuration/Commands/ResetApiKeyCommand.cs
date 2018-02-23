using Radarr.Core.Messaging.Commands;

namespace Radarr.Core.Configuration.Commands
{
    public class ResetApiKeyCommand : Command
    {
        public override bool SendUpdatesToClient => true;
    }
}
