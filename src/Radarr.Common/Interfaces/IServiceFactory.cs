using System;
using System.Collections.Generic;

namespace Radarr.Common.Interfaces
{
    public interface IServiceFactory
    {
        T Build<T>() where T : class;

        IEnumerable<T> BuildAll<T>() where T : class;

        object Build(Type contract);

        IEnumerable<Type> GetImplementations(Type contract);
    }
}
