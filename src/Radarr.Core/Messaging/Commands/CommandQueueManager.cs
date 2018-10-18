using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NLog;
using Radarr.Common.Cache.Interfaces;
using Radarr.Common.EnsureThat;
using Radarr.Common.Interfaces;
using Radarr.Common.Serializer;
using Radarr.Core.Datastore.Databases.Main.Models;
using Radarr.Core.Datastore.Databases.Main.Repositories.Interfaces;
using Radarr.Core.Lifecycle.Events;
using Radarr.Core.Messaging.Commands.Interfaces;
using Radarr.Core.Messaging.Events.Interfaces;

namespace Radarr.Core.Messaging.Commands
{
    public class CommandQueueManager : ICommandQueueManager, IHandle<ApplicationStartedEvent>
    {
        private readonly ICommandRepository _repository;

        private readonly IServiceFactory _serviceFactory;

        private readonly Logger _logger;

        private readonly ICached<CommandModel> _commandCache;

        private readonly BlockingCollection<CommandModel> _commandQueue; 

        public CommandQueueManager(ICommandRepository repository, 
                                   IServiceFactory serviceFactory,
                                   ICacheManager cacheManager,
                                   Logger logger)
        {
            _repository = repository;
            _serviceFactory = serviceFactory;
            _logger = logger;

            _commandCache = cacheManager.GetCache<CommandModel>(GetType());
            _commandQueue = new BlockingCollection<CommandModel>(new CommandQueue());
        }

        public CommandModel Push<TCommand>(TCommand command, CommandPriority priority = CommandPriority.Normal, CommandTrigger trigger = CommandTrigger.Unspecified) where TCommand : Command
        {
            Ensure.That(command, () => command).IsNotNull();

            _logger.Trace("Publishing {0}", command.Name);
            _logger.Trace("Checking if command is queued or started: {0}", command.Name);

            var existingCommands = QueuedOrStarted(command.Name);
            var existing = existingCommands.SingleOrDefault(c => CommandEqualityComparer.Instance.Equals(c.BodyObj, command));

            if (existing != null)
            {
                _logger.Trace("Command is already in progress: {0}", command.Name);

                return existing;
            }

            var commandModel = new CommandModel
            {
                Name = command.Name,
                BodyObj = command,
                QueuedAt = DateTime.UtcNow,
                Trigger = trigger,
                Priority = priority,
                Status = CommandStatus.Queued
            };

            _logger.Trace("Inserting new command: {0}", commandModel.Name);

            _repository.Insert(commandModel);
            _commandCache.Set(commandModel.Id.ToString(), commandModel);
            _commandQueue.Add(commandModel);

            return commandModel;
        }

        public CommandModel Push(string commandName, DateTime? lastExecutionTime, CommandPriority priority = CommandPriority.Normal, CommandTrigger trigger = CommandTrigger.Unspecified)
        {
            dynamic command = GetCommand(commandName);
            command.LastExecutionTime = lastExecutionTime;
            command.Trigger = trigger;

            return Push(command, priority, trigger);
        }

        public IEnumerable<CommandModel> Queue(CancellationToken cancellationToken)
        {
            return _commandQueue.GetConsumingEnumerable(cancellationToken);
        }

        public CommandModel Get(int id)
        {
            return _commandCache.Get(id.ToString(), () => FindCommand(_repository.Get(id)));
        }

        public List<CommandModel> GetStarted()
        {
            _logger.Trace("Getting started commands");
            return _commandCache.Values.Where(c => c.Status == CommandStatus.Started).ToList();
        }

        public void SetMessage(CommandModel command, string message)
        {
            command.Message = message;
            _commandCache.Set(command.Id.ToString(), command);
        }

        public void Start(CommandModel command)
        {
            command.StartedAt = DateTime.UtcNow;
            command.Status = CommandStatus.Started;

            _logger.Trace("Marking command as started: {0}", command.Name);
            _commandCache.Set(command.Id.ToString(), command);         
            _repository.Start(command);
        }

        public void Complete(CommandModel command, string message)
        {
            Update(command, CommandStatus.Completed, message);
        }

        public void Fail(CommandModel command, string message, Exception e)
        {
            command.Exception = e.ToString();
            
            Update(command, CommandStatus.Failed, message);
        }

        public void Requeue()
        {
            foreach (var command in _repository.Queued())
            {
                _commandQueue.Add(command);
            }
        }

        public void CleanCommands()
        {
            _logger.Trace("Cleaning up old commands");
            
            var old = _commandCache.Values.Where(c => c.EndedAt < DateTime.UtcNow.AddMinutes(-5));

            foreach (var command in old)
            {
                _commandCache.Remove(command.Id.ToString());
            }

            _repository.Trim();
        }

        private dynamic GetCommand(string commandName)
        {
            commandName = commandName.Split('.').Last();

            var commandType = _serviceFactory.GetImplementations(typeof(Command))
                                             .Single(c => c.Name.Equals(commandName, StringComparison.InvariantCultureIgnoreCase));

            return Json.Deserialize("{}", commandType);
        }

        private CommandModel FindCommand(CommandModel command)
        {
            var cachedCommand = _commandCache.Find(command.Id.ToString());

            if (cachedCommand != null)
            {
                command.Message = cachedCommand.Message;
            }

            return command;
        }

        private void Update(CommandModel command, CommandStatus status, string message)
        {
            SetMessage(command, message);

            command.EndedAt = DateTime.UtcNow;
            command.Duration = command.EndedAt.Value.Subtract(command.StartedAt.Value);
            command.Status = status;

            _logger.Trace("Updating command status");
            _commandCache.Set(command.Id.ToString(), command);
            _repository.End(command);
        }

        private List<CommandModel> QueuedOrStarted(string name)
        {
            return _commandCache.Values.Where(q => q.Name == name &&
                                                   (q.Status == CommandStatus.Queued ||
                                                    q.Status == CommandStatus.Started)).ToList();
        }

        public void Handle(ApplicationStartedEvent message)
        {
            _logger.Trace("Orphaning incomplete commands");
            _repository.OrphanStarted();
            Requeue();
        }
    }
}
