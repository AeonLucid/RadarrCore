using System;
using System.IO;
using NLog;
using Radarr.Common.EnvironmentInfo;
using Radarr.Common.EnvironmentInfo.Interfaces;
using Radarr.Common.Processes.Interfaces;

namespace Radarr.Common.Processes
{
    public class PidFileProvider : IPidFileProvider
    {
        private readonly IAppFolderInfo _appFolderInfo;

        private readonly IProcessProvider _processProvider;

        private readonly ILogger _logger;

        public PidFileProvider(IAppFolderInfo appFolderInfo, IProcessProvider processProvider, ILogger logger)
        {
            _appFolderInfo = appFolderInfo;
            _processProvider = processProvider;
            _logger = logger;
        }

        public void Write()
        {
            if (OsInfoCore.IsWindows)
            {
                return;
            }

            var filename = Path.Combine(_appFolderInfo.AppDataFolder, "radarr.pid");
            try
            {
                File.WriteAllText(filename, _processProvider.GetCurrentProcessId().ToString());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to write PID file: " + filename);
                throw;
            }
        }
    }
}
