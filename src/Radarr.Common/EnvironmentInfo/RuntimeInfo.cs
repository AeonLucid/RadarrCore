using System;
using Radarr.Common.EnvironmentInfo.Interfaces;

namespace Radarr.Common.EnvironmentInfo
{
    public class RuntimeInfo : IRuntimeInfo
    {
        public static bool IsUserInteractive => Environment.UserInteractive;

        bool IRuntimeInfo.IsUserInteractive => IsUserInteractive;

        public bool IsAdmin { get; }

        public bool IsWindowsService { get; }

        public bool IsConsole { get; }

        public bool IsRunning { get; set; }

        public bool RestartPending { get; set; }

        public string ExecutingApplication { get; }

        public string RuntimeVersion { get; }
    }
}
