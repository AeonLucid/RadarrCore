using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Radarr.Common.Messaging;
using Radarr.Core.Messaging.Commands;

namespace Radarr.Core.Datastore.Databases.Main.Models
{
    public class CommandModel : ModelBase, IMessage
    {
        public string Name { get; set; }

        [NotMapped]
        public Command BodyObj
        {
            get => (Body == null) ? null : JsonConvert.DeserializeObject<Command>(Body);
            set => Body = JsonConvert.SerializeObject(value);
        }

        public string Body { get; set; }

        public CommandPriority Priority { get; set; }

        public CommandStatus Status { get; set; }

        public DateTime QueuedAt { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? EndedAt { get; set; }

        public TimeSpan? Duration { get; set; }

        public string Exception { get; set; }

        public CommandTrigger Trigger { get; set; }

        public string Message { get; set; }
    }
}
