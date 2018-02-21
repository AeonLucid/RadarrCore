using System.IO;
using Radarr.Common.EnvironmentInfo;

namespace Radarr.Test.Common
{
    public static class StringExtensions
    {
        public static string AsOsAgnostic(this string path)
        {
            if (!OsInfoCore.IsWindows)
            {
                if (path.Length > 2 && path[1] == ':')
                {
                    path = path.Replace(":", "");
                    path = Path.DirectorySeparatorChar + path;
                }
                path = path.Replace("\\", Path.DirectorySeparatorChar.ToString());
            }

            return path;
        }
    }
}
