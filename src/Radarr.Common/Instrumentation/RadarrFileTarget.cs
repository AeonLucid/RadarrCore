using NLog;
using NLog.Targets;

namespace Radarr.Common.Instrumentation
{
    public class RadarrFileTarget : FileTarget
    {
        protected override string GetFormattedMessage(LogEventInfo logEvent)
        {
            return CleanseLogMessage.Cleanse(Layout.Render(logEvent));
        }
    }
}
