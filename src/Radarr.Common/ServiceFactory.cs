using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Radarr.Common.Extensions;
using Radarr.Common.Interfaces;

namespace Radarr.Common
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly IContainer _container;

        public ServiceFactory(IContainer container)
        {
            _container = container;
        }

        public T Build<T>() where T : class
        {
            return _container.Resolve<T>();
        }

        public IEnumerable<T> BuildAll<T>() where T : class
        {
            // TODO: Verify this works.
            var types = _container.ComponentRegistry.Registrations
                .Where(r => typeof(T).IsAssignableFrom(r.Activator.LimitType))
                .Select(r => r.Activator.LimitType);

            return types.Select(t => _container.Resolve(t) as T);
        }

        public object Build(Type contract)
        {
            return _container.Resolve(contract);
        }

        public IEnumerable<Type> GetImplementations(Type contract)
        {
            return _container.GetImplementations(contract);
        }
    }
}
