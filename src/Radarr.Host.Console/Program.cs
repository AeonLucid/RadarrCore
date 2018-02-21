using Radarr.Common.EnvironmentInfo;
using Radarr.Common.Instrumentation;

namespace Radarr.Host.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var startupArgs = new StartupContext(args);
            RadarrInstrumentation.Register(startupArgs, false, true);

            var bootstrap = new Bootstrap(startupArgs);
            bootstrap.Start();

            System.Console.ReadKey();
        }
    }
}
