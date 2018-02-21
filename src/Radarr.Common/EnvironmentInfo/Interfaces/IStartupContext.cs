using System.Collections.Generic;

namespace Radarr.Common.EnvironmentInfo.Interfaces
{
    public interface IStartupContext
    {
        HashSet<string> Flags { get; }

        Dictionary<string, string> Args { get; }

        bool InstallService { get; }

        bool UninstallService { get; }

        string PreservedArguments { get; }
    }
}