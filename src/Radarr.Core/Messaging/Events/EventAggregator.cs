using System;
using System.Threading.Tasks;
using NLog;
using Radarr.Common.EnsureThat;
using Radarr.Common.Interfaces;
using Radarr.Common.Messaging;
using Radarr.Common.TPL;
using Radarr.Core.Messaging.Events.Interfaces;

namespace Radarr.Core.Messaging.Events
{
    public class EventAggregator : IEventAggregator
    {
        private readonly ILogger _logger;

        private readonly IServiceFactory _serviceFactory;

        private readonly TaskFactory _taskFactory;

        public EventAggregator(ILogger logger, IServiceFactory serviceFactory)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
            _taskFactory = new TaskFactory();
        }

        public void PublishEvent<TEvent>(TEvent @event) where TEvent : class, IEvent
        {
            Ensure.That(@event, () => @event).IsNotNull();

            var eventName = GetEventName(@event.GetType());

            _logger.Trace("Publishing {0}", eventName);

            // Call synchronous handlers.
            foreach (var handler in _serviceFactory.BuildAll<IHandle<TEvent>>())
            {
                try
                {
                    _logger.Trace("{0} -> {1}", eventName, handler.GetType().Name);

                    handler.Handle(@event);

                    _logger.Trace("{0} <- {1}", eventName, handler.GetType().Name);
                }
                catch (Exception e)
                {
                    _logger.Error(e,string.Format("{0} failed while processing [{1}]", handler.GetType().Name, eventName));
                }
            }

            // Call asynchronous handlers.
            foreach (var handler in _serviceFactory.BuildAll<IHandleAsync<TEvent>>())
            {
                var handlerLocal = handler;

                _taskFactory.StartNew(() => {
                    _logger.Trace("{0} ~> {1}", eventName, handlerLocal.GetType().Name);

                    handlerLocal.HandleAsync(@event);

                    _logger.Trace("{0} <~ {1}", eventName, handlerLocal.GetType().Name);
                }, TaskCreationOptions.PreferFairness).LogExceptions();
            }
        }

        private static string GetEventName(Type eventType)
        {
            if (!eventType.IsGenericType)
            {
                return eventType.Name;
            }

            return string.Format("{0}<{1}>", eventType.Name.Remove(eventType.Name.IndexOf('`')), eventType.GetGenericArguments()[0].Name);
        }
    }
}