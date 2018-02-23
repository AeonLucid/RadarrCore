using Radarr.Common.Messaging;

namespace Radarr.Core.Messaging.Events.Interfaces
{
    public interface IEventAggregator
    {
        void PublishEvent<TEvent>(TEvent @event) where TEvent : class, IEvent;
    }
}