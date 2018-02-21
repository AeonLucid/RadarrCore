using System.Linq;
using Autofac;
using Autofac.Core;
using NLog;
using Radarr.Common.Instrumentation;

namespace Radarr.Host.Autofac
{
    public class LoggingModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            registration.Preparing += RegistrationOnPreparing;
        }

        private void RegistrationOnPreparing(object sender, PreparingEventArgs e)
        {
            var limitType = e.Component.Activator.LimitType;

            e.Parameters = e.Parameters.Union(new[]
            {
                new ResolvedParameter(
                    (pi, c) => pi.ParameterType == typeof(ILogger),
                    (pi, c) => RadarrLogger.GetLogger(limitType))
            });
        }
    }
}
