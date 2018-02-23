using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Radarr.Core.Datastore.Base
{
    public interface IBasicRepository<TModel> where TModel : ModelBase, new()
    {
        IEnumerable<TModel> All();

        int Count();

        TModel Get(int id);

        IEnumerable<TModel> Get(IEnumerable<int> ids);

        TModel SingleOrDefault();

        TModel Insert(TModel model);

        TModel Update(TModel model);

        TModel Upsert(TModel model);

        void Delete(int id);

        void Delete(TModel model);

        void InsertMany(IList<TModel> model);

        void UpdateMany(IList<TModel> model);

        void DeleteMany(List<TModel> model);

        void Purge(bool vacuum = false);

        bool HasItems();

        void DeleteMany(IEnumerable<int> ids);

        void SetFields(TModel model, params Expression<Func<TModel, object>>[] properties);

        TModel Single();

        PagingSpec<TModel> GetPaged(PagingSpec<TModel> pagingSpec);
    }
}
