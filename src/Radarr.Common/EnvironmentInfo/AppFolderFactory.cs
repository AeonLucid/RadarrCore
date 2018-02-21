using System;
using System.Security.AccessControl;
using System.Security.Principal;
using NLog;
using Radarr.Common.Disk.Interfaces;
using Radarr.Common.EnvironmentInfo.Interfaces;
using Radarr.Common.Instrumentation;

namespace Radarr.Common.EnvironmentInfo
{
    public class AppFolderFactory : IAppFolderFactory
    {
        private readonly IAppFolderInfo _appFolderInfo;

        private readonly IDiskProvider _diskProvider;

        private readonly Logger _logger;

        public AppFolderFactory(IAppFolderInfo appFolderInfo, IDiskProvider diskProvider)
        {
            _appFolderInfo = appFolderInfo;
            _diskProvider = diskProvider;
            _logger = RadarrLogger.GetLogger(this);
        }

        public void Register()
        {
            _diskProvider.EnsureFolder(_appFolderInfo.AppDataFolder);

            if (OsInfoCore.IsWindows)
            {
                SetPermissions();
            }
        }

        private void SetPermissions()
        {
            try
            {
                _diskProvider.SetPermissions(_appFolderInfo.AppDataFolder, WellKnownSidType.WorldSid, FileSystemRights.Modify, AccessControlType.Allow);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Coudn't set app folder permission");
            }
        }
    }
}