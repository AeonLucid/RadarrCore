using Radarr.Common.EnvironmentInfo;
using Radarr.Common.EnvironmentInfo.Interfaces;

namespace Radarr.Common.Instrumentation
{
    public static class RadarrInstrumentation
    {
        public static void Register(IStartupContext startupArgs, bool updateApp, bool inConsole)
        {
            RadarrLogger.Register(startupArgs, updateApp, inConsole);
            GlobalExceptionHandlers.Register();
        }
    }
}
