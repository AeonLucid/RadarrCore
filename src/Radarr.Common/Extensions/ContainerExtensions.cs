using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Radarr.Common.Composition;

namespace Radarr.Common.Extensions
{
    public static class ContainerExtensions
    {
        public static IEnumerable<Type> GetImplementations(this IContainer container, Type contractType)
        {
            var metadata = container.Resolve<ContainerMetadata>();

            return metadata.LoadedTypes.Where(implementation =>
                contractType.IsAssignableFrom(implementation) &&
                !implementation.IsInterface &&
                !implementation.IsAbstract
            );
        }
    }
}