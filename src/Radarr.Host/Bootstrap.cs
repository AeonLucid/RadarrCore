using System;
using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Radarr.Common.EnvironmentInfo;
using Radarr.Common.EnvironmentInfo.Interfaces;
using Radarr.Common.Instrumentation;
using Radarr.Common.Processes.Interfaces;

namespace Radarr.Host
{
    public class Bootstrap
    {
        private static readonly Logger Logger = RadarrLogger.GetLogger(typeof(Bootstrap));

        private readonly IStartupContext _startupContext;

        private readonly IContainer _container;

        private readonly ApplicationModes _appMode;

        public Bootstrap(IStartupContext startupContext)
        {
            _startupContext = startupContext;
            _container = MainAppContainerBuilder.BuildContainer(startupContext);
            _container.Resolve<IAppFolderFactory>().Register();
            _container.Resolve<IPidFileProvider>().Write();
            _appMode = GetApplicationMode();
        }

        public void Start(Action<IContainer> startCallback = null)
        {
            NLogBuilder.ConfigureNLog(LogManager.Configuration);

            try
            {
                Logger.Info("Starting Radarr - {0} - Version {1}", Assembly.GetCallingAssembly().Location, Assembly.GetExecutingAssembly().GetName().Version);
                
                StartApp();

                // TODO: _container.Resolve<ICancelHandler>().Attach();

                if (startCallback != null)
                {
                    startCallback(_container);
                }
                else
                {
                    SpinToExit();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Radarr.Host startup failed because of an exception");
                throw;
            }
        }

        private void StartApp()
        {
            // tmp
            _container.Resolve<IRuntimeInfo>().IsRunning = true;

            var webhost = new WebHostBuilder()
                .UseKestrel(x => x.AddServerHeader = false)
                .UseStartup<BootstrapStartup>()
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                })
                .UseNLog()
                .Build();

            webhost.RunAsync();
        }

        private void SpinToExit()
        {
            if (IsInUtilityMode())
            {
                return;
            }

            _container.Resolve<IWaitForExit>().Spin();
        }

        private ApplicationModes GetApplicationMode()
        {
            if (_startupContext.Flags.Contains(StartupContext.HELP))
            {
                return ApplicationModes.Help;
            }

            if (OsInfoCore.IsWindows && _startupContext.InstallService)
            {
                return ApplicationModes.InstallService;
            }

            if (OsInfoCore.IsWindows && _startupContext.UninstallService)
            {
                return ApplicationModes.UninstallService;
            }

            if (_container.Resolve<IRuntimeInfo>().IsWindowsService)
            {
                return ApplicationModes.Service;
            }

            return ApplicationModes.Interactive;
        }

        private bool IsInUtilityMode()
        {
            switch (_appMode)
            {
                case ApplicationModes.InstallService:
                case ApplicationModes.UninstallService:
                case ApplicationModes.Help:
                {
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }
    }
}
