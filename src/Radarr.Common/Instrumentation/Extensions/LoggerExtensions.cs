using NLog;

namespace Radarr.Common.Instrumentation.Extensions
{
    public static class LoggerExtensions
    {
        public static void ProgressInfo(this ILogger logger, string message, params object[] args)
        {
            var formattedMessage = string.Format(message, args);
            LogProgressMessage(logger, LogLevel.Info, formattedMessage);
        }

        public static void ProgressDebug(this ILogger logger, string message, params object[] args)
        {
            var formattedMessage = string.Format(message, args);
            LogProgressMessage(logger, LogLevel.Debug, formattedMessage);
        }

        public static void ProgressTrace(this ILogger logger, string message, params object[] args)
        {
            var formattedMessage = string.Format(message, args);
            LogProgressMessage(logger, LogLevel.Trace, formattedMessage);
        }

        private static void LogProgressMessage(ILogger logger, LogLevel level, string message)
        {
            var logEvent = new LogEventInfo(level, logger.Name, message);
            logEvent.Properties.Add("Status", "");

            logger.Log(logEvent);
        }
    }
}
