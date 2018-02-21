using System;
using System.IO;
using System.Reflection;
using NLog;
using Radarr.Common.EnvironmentInfo.Interfaces;
using Radarr.Common.Instrumentation;

namespace Radarr.Common.EnvironmentInfo
{
    public class AppFolderInfo : IAppFolderInfo
    {
        private static readonly Logger Logger = RadarrLogger.GetLogger(typeof(AppFolderInfo));

        private static readonly Environment.SpecialFolder DataSpecialFolder = Environment.SpecialFolder.CommonApplicationData;

        public AppFolderInfo(IStartupContext startupContext)
        {
            if (startupContext.Args.ContainsKey(StartupContext.APPDATA))
            {
                AppDataFolder = startupContext.Args[StartupContext.APPDATA];
                Logger.Info("Data directory is being overridden to [{0}]", AppDataFolder);
            }
            else
            {
                AppDataFolder = Path.Combine(Environment.GetFolderPath(DataSpecialFolder, Environment.SpecialFolderOption.None), "Radarr");
            }

            StartUpFolder = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
            TempFolder = Path.GetTempPath();
        }

        public string AppDataFolder { get; }

        public string TempFolder { get; }

        public string StartUpFolder { get; }
    }
}
