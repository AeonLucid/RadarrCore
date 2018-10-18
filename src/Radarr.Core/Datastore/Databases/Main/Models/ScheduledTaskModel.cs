using System;

namespace Radarr.Core.Datastore.Databases.Main.Models
{
    public class ScheduledTaskModel : ModelBase
    {
        public string TypeName { get; set; }

        public double Interval { get; set; }

        public DateTime LastExecution { get; set; }
    }
}
