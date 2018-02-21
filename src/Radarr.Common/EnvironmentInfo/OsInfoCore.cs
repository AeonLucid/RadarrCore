using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Radarr.Common.EnvironmentInfo
{
    public static class OsInfoCore
    {
        static OsInfoCore()
        {
            IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            IsOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            FirstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            PathStringComparison = IsWindows
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
        }

        public static bool IsWindows { get; }

        public static bool IsOSX { get; }

        public static bool IsLinux { get; }

        public static DayOfWeek FirstDayOfWeek { get; set; }

        public static StringComparison PathStringComparison { get; set; }
    }
}
