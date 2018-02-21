using System.Collections.Generic;
using Autofac;
using Radarr.Common.Composition;
using Radarr.Common.EnvironmentInfo;
using Radarr.Common.EnvironmentInfo.Interfaces;

namespace Radarr.Host
{
    public class MainAppContainerBuilder : ContainerBuilderBase
    {
        private MainAppContainerBuilder(IStartupContext startupContext, string[] assemblies) : base(startupContext, assemblies)
        {
            
        }

        public static IContainer BuildContainer(IStartupContext startupContext)
        {
            var assemblies = new List<string>
            {
                "Radarr.Host",
                "Radarr.Common",
//                "NzbDrone.Core",
//                "NzbDrone.Api",
//                "NzbDrone.SignalR"
            };

            if (OsInfoCore.IsWindows)
            {
                assemblies.Add("Radarr.Runtime.Windows");
            }

            return new MainAppContainerBuilder(startupContext, assemblies.ToArray()).Container;
        }
    }
}
