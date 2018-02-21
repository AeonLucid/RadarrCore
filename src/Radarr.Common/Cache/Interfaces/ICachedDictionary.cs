﻿using System;
using System.Collections.Generic;

namespace Radarr.Common.Cache.Interfaces
{
    public interface ICachedDictionary<TValue> : ICached
    {
        void RefreshIfExpired();

        void RefreshIfExpired(TimeSpan ttl);

        void Refresh();

        void Update(IDictionary<string, TValue> items);

        void ExtendTTL();

        TValue Get(string key);

        TValue Find(string key);

        bool IsExpired(TimeSpan ttl);
    }
}
