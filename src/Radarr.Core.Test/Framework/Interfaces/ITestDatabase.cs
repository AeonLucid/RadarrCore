using System.Collections.Generic;
using Radarr.Core.Datastore;

namespace Radarr.Core.Test.Framework.Interfaces
{
    public interface ITestDatabase
    {
        void InsertMany<T>(IEnumerable<T> items) where T : ModelBase, new();

        T Insert<T>(T item) where T : ModelBase, new();

        List<T> All<T>() where T : ModelBase, new();

        T Single<T>() where T : ModelBase, new();

        void Update<T>(T childModel) where T : ModelBase, new();

        void Delete<T>(T childModel) where T : ModelBase, new();
    }
}
