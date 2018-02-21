using System.Threading;
using NLog;
using Radarr.Common.EnvironmentInfo.Interfaces;
using Radarr.Common.Processes.Interfaces;

namespace Radarr.Host
{
    public class WaitForExit : IWaitForExit
    {
        private readonly IRuntimeInfo _runtimeInfo;

        private readonly IProcessProvider _processProvider;

        private readonly IStartupContext _startupContext;

        private readonly ILogger _logger;

        public WaitForExit(IRuntimeInfo runtimeInfo, IProcessProvider processProvider, IStartupContext startupContext, ILogger logger)
        {
            _runtimeInfo = runtimeInfo;
            _processProvider = processProvider;
            _startupContext = startupContext;
            _logger = logger;
        }

        public void Spin()
        {
            while (_runtimeInfo.IsRunning)
            {
                Thread.Sleep(1000);
            }

            _logger.Debug("Wait loop was terminated.");

            if (_runtimeInfo.RestartPending)
            {
                var restartArgs = GetRestartArgs();

                _logger.Info("Attempting restart with arguments: {0}", restartArgs);
                _processProvider.SpawnNewProcess(_runtimeInfo.ExecutingApplication, restartArgs);
            }
        }

        private string GetRestartArgs()
        {
            var args = _startupContext.PreservedArguments;

            args += " /restart";

            if (!args.Contains("/nobrowser"))
            {
                args += " /nobrowser";
            }

            return args;
        }
    }
}
