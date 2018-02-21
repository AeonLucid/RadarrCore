using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using NLog;
using Radarr.Common.EnvironmentInfo.Interfaces;
using Radarr.Common.Instrumentation;
using Radarr.Common.Messaging;
using Radarr.Host.Autofac;

namespace Radarr.Common.Composition
{
    public abstract class ContainerBuilderBase
    {
        private readonly ContainerBuilder _containerBuilder;

        private readonly List<Type> _loadedTypes;

        protected ContainerBuilderBase(IStartupContext startupContext, params string[] assemblies)
        {
            _containerBuilder = new ContainerBuilder();
            _containerBuilder.RegisterModule<LoggingModule>();
            _containerBuilder.RegisterInstance(startupContext).SingleInstance();
            _containerBuilder.RegisterType<ILogger>();
            _loadedTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                _loadedTypes.AddRange(Assembly.Load(assembly).GetTypes());
            }

            AutoRegisterInterfaces();

            Container = _containerBuilder.Build();
        }

        public IContainer Container { get; }

        private void AutoRegisterInterfaces()
        {
            var loadedInterfaces = _loadedTypes.Where(t => t.IsInterface).ToList();
            var implementedInterfaces = _loadedTypes.SelectMany(t => t.GetInterfaces());

            var contracts = loadedInterfaces.Union(implementedInterfaces).Where(c => !c.IsGenericTypeDefinition && !string.IsNullOrWhiteSpace(c.FullName))
                .Where(c => !c.FullName.StartsWith("System"))
                .Except(new List<Type> { typeof(IMessage), typeof(IEvent), typeof(IContainer) }).Distinct().OrderBy(c => c.FullName);

            foreach (var contract in contracts)
            {
                AutoRegisterImplementations(contract);
            }
        }

        protected void AutoRegisterImplementations<TContract>()
        {
            AutoRegisterImplementations(typeof(TContract));
        }

        private void AutoRegisterImplementations(Type contractType)
        {
            var implementations = _loadedTypes.Where(implementation =>
                contractType.IsAssignableFrom(implementation) &&
                !implementation.IsInterface &&
                !implementation.IsAbstract
            ).ToList();

            switch (implementations.Count)
            {
                case 0:
                    return;
                case 1:
                    _containerBuilder.RegisterType(implementations.Single())
                        .AsImplementedInterfaces()
                        .SingleInstance();
                    break;
                default:
                    _containerBuilder.RegisterTypes(implementations.ToArray())
                        .AsImplementedInterfaces()
                        .SingleInstance();
                    break;
            }
        }
    }
}