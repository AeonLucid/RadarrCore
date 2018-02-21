using Radarr.Common.Messaging;

namespace Radarr.Core.Messaging.Events
{
    public interface IEventAggregator
    {
        void PublishEvent<TEvent>(TEvent @event) where TEvent : class, IEvent;
    }
}
