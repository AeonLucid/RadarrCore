using System.ServiceProcess;
using Radarr.Common.EnvironmentInfo.Interfaces;

namespace Radarr.Host
{
    public class ApplicationServer : ServiceBase
    {
        private readonly IRuntimeInfo _runtimeInfo;

        public ApplicationServer(IRuntimeInfo runtimeInfo)
        {
            _runtimeInfo = runtimeInfo;
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        public void Start()
        {
            // TODO: DO
        }

        public void Shutdown()
        {
            _runtimeInfo.IsRunning = false;
        }
    }
}
