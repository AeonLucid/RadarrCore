﻿using System;
using System.Collections.Generic;
using System.Text;
using Radarr.Common.Cache.Interfaces;
using Radarr.Common.EnsureThat;

namespace Radarr.Common.Cache
{
    public class CacheManager : ICacheManager
    {
        private readonly ICached<ICached> _cache;

        public CacheManager()
        {
            _cache = new Cached<ICached>();

        }

        public void Clear()
        {
            _cache.Clear();
        }

        public ICollection<ICached> Caches => _cache.Values;

        public ICached<T> GetCache<T>(Type host)
        {
            Ensure.That(host, () => host).IsNotNull();
            return GetCache<T>(host, host.FullName);
        }

        public ICached<T> GetCache<T>(Type host, string name)
        {
            Ensure.That(host, () => host).IsNotNull();
            Ensure.That(name, () => name).IsNotNullOrWhiteSpace();

            return (ICached<T>)_cache.Get(host.FullName + "_" + name, () => new Cached<T>());
        }

        public ICachedDictionary<T> GetCacheDictionary<T>(Type host, string name, Func<IDictionary<string, T>> fetchFunc = null, TimeSpan? lifeTime = null)
        {
            Ensure.That(host, () => host).IsNotNull();
            Ensure.That(name, () => name).IsNotNullOrWhiteSpace();

            return (ICachedDictionary<T>)_cache.Get("dict_" + host.FullName + "_" + name, () => new CachedDictionary<T>(fetchFunc, lifeTime));
        }
    }
}
