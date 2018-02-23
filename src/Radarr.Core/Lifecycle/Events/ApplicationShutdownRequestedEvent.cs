using Radarr.Common.Messaging;

namespace Radarr.Core.Lifecycle.Events
{
    public class ApplicationShutdownRequestedEvent : IEvent
    {
        public bool Restarting { get; set; }

        public ApplicationShutdownRequestedEvent()
        {
        }

        public ApplicationShutdownRequestedEvent(bool restarting)
        {
            Restarting = restarting;
        }
    }
}
