using System;
using System.Threading;
using NLog;
using Radarr.Common.Interfaces;
using Radarr.Core.Datastore.Main.Models;
using Radarr.Core.Lifecycle.Events;
using Radarr.Core.Messaging.Commands.Events;
using Radarr.Core.Messaging.Commands.Exceptions;
using Radarr.Core.Messaging.Commands.Interfaces;
using Radarr.Core.Messaging.Events;
using Radarr.Core.Messaging.Events.Interfaces;
using Radarr.Core.ProgressMessaging;

namespace Radarr.Core.Messaging.Commands
{
    public class CommandExecutor : IHandle<ApplicationStartedEvent>,
                                   IHandle<ApplicationShutdownRequestedEvent>
    {
        private readonly Logger _logger;

        private readonly IServiceFactory _serviceFactory;

        private readonly ICommandQueueManager _commandQueueManager;

        private readonly IEventAggregator _eventAggregator;

        private static CancellationTokenSource _cancellationTokenSource;

        private const int THREAD_LIMIT = 3;

        public CommandExecutor(IServiceFactory serviceFactory,
                               ICommandQueueManager commandQueueManager,
                               IEventAggregator eventAggregator,
                               Logger logger)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
            _commandQueueManager = commandQueueManager;
            _eventAggregator = eventAggregator;
        }

        private void ExecuteCommands()
        {
            try
            {
                foreach (var command in _commandQueueManager.Queue(_cancellationTokenSource.Token))
                {
                    try
                    {
                        ExecuteCommand((dynamic) command.Body, command);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Error occurred while executing task " + command.Name);
                    }
                }
            }
            catch (ThreadAbortException ex)
            {
                _logger.Error(ex, "Thread aborted: " + ex.Message);
                Thread.ResetAbort();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unknown error in thread: " + ex.Message);
            }
        }

        private void ExecuteCommand<TCommand>(TCommand command, CommandModel commandModel) where TCommand : Command
        {
            var handlerContract = typeof(IExecute<>).MakeGenericType(command.GetType());
            var handler = (IExecute<TCommand>)_serviceFactory.Build(handlerContract);

            _logger.Trace("{0} -> {1}", command.GetType().Name, handler.GetType().Name);

            try
            {
                _commandQueueManager.Start(commandModel);
                BroadcastCommandUpdate(commandModel);

                if (ProgressMessageContext.CommandModel == null)
                {
                    ProgressMessageContext.CommandModel = commandModel;
                }

                handler.Execute(command);

                _commandQueueManager.Complete(commandModel, command.CompletionMessage);
            }
            catch (CommandFailedException ex)
            {
                _commandQueueManager.SetMessage(commandModel, "Failed");
                _commandQueueManager.Fail(commandModel, ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                _commandQueueManager.SetMessage(commandModel, "Failed");
                _commandQueueManager.Fail(commandModel, "Failed", ex);
                throw;
            }
            finally
            {
                BroadcastCommandUpdate(commandModel);

                _eventAggregator.PublishEvent(new CommandExecutedEvent(commandModel));

                if (ProgressMessageContext.CommandModel == commandModel)
                {
                    ProgressMessageContext.CommandModel = null;
                }
            }

            _logger.Trace("{0} <- {1} [{2}]", command.GetType().Name, handler.GetType().Name, commandModel.Duration.ToString());
        }
        
        private void BroadcastCommandUpdate(CommandModel command)
        {
            if (command.Body.SendUpdatesToClient)
            {
                _eventAggregator.PublishEvent(new CommandUpdatedEvent(command));
            }
        }

        public void Handle(ApplicationStartedEvent message)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            for (int i = 0; i < THREAD_LIMIT; i++)
            {
                var thread = new Thread(ExecuteCommands);
                thread.Start();
            }
        }

        public void Handle(ApplicationShutdownRequestedEvent message)
        {
            _logger.Info("Shutting down task execution");
            _cancellationTokenSource.Cancel(true);
        }
    }
}