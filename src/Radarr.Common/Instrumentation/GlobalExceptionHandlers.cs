using System;
using System.Threading.Tasks;
using NLog;

namespace Radarr.Common.Instrumentation
{
    public static class GlobalExceptionHandlers
    {
        private static readonly Logger Logger = RadarrLogger.GetLogger(typeof(GlobalExceptionHandlers));

        public static void Register()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleAppDomainException;
            TaskScheduler.UnobservedTaskException += HandleTaskException;
        }

        private static void HandleTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var exception = e.Exception;
            
            Logger.Error(exception, "Task error: " + exception.Message);
        }

        private static void HandleAppDomainException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!(e.ExceptionObject is Exception exception)) return;

            Logger.Fatal(exception, "Epic failure: " + exception.Message);
        }
    }
}
