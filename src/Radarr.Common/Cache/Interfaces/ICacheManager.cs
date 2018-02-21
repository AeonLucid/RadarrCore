using System;
using System.Collections.Generic;

namespace Radarr.Common.Cache.Interfaces
{
    public interface ICacheManager
    {
        ICached<T> GetCache<T>(Type host);

        ICached<T> GetCache<T>(Type host, string name);

        ICachedDictionary<T> GetCacheDictionary<T>(Type host, string name, Func<IDictionary<string, T>> fetchFunc = null, TimeSpan? lifeTime = null);

        void Clear();

        ICollection<ICached> Caches { get; }
    }
}
